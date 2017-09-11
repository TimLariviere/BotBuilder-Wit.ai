using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Framework.Builder.Exceptions;
using Microsoft.Bot.Framework.Builder.Witai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.Bot.Framework.Builder.Witai.Dialogs
{
    public delegate Task IntentHandler(IDialogContext context, WitResult witResult);

    public delegate Task IntentActivityHandler(IDialogContext context, IAwaitable<IMessageActivity> message, WitResult witResult);

    [Serializable]
    public class WitDialog<TResult> : IDialog<TResult>
    {
        protected readonly IWitService service;
       
        [NonSerialized]
        protected Dictionary<string, IntentActivityHandler> handlerByIntent;
        protected IWitContext WitContext;
        private string WitThreadId;

        public IWitService MakeServiceFromAttributes()
        {
            var type = this.GetType();
            var witModels = type.GetCustomAttributes<WitModelAttribute>(inherit: true);
            if (witModels.ToArray().Length > 1)
            {
                throw new WitModelDisambiguationException("WitDialog does not support more than one WitModel per instance");
            }

            return new WitService(witModels.ToArray()[0]);
        }

        public WitDialog()
        {
            SetField.NotNull(out this.service, nameof(service), this.MakeServiceFromAttributes());
            this.StartNewThread();
        }

        public WitDialog(IWitService service)
        {
            SetField.NotNull(out this.service, nameof(service), service);
            this.StartNewThread();
        }

        protected void StartNewThread()
        {
            this.WitContext = new WitContext();
            this.WitThreadId = Guid.NewGuid().ToString();
        }

        public virtual Task StartAsync(IDialogContext context)
        {            
            context.Wait(MessageReceived);
            return Task.CompletedTask;
        }
        
        protected virtual async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            await MessageHandler(context, item);
        }

        private async Task MessageHandler(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var message = await item;
            var messageText = await GetWitQueryTextAsync(context, message);
            string jsonContext = this.WitContext.ToJsonString();
            var result = await this.service.QueryAsync(messageText, this.WitThreadId, jsonContext, context.CancellationToken);
                
            await DispatchToIntentHandler(context, item, result);
        }

        protected virtual async Task DispatchToIntentHandler(IDialogContext context, IAwaitable<IMessageActivity> item, WitResult result)
        {
            if (this.handlerByIntent == null)
            {
                this.handlerByIntent = new Dictionary<string, IntentActivityHandler>(GetHandlersByIntent());
            }

            var intent = result.Entities.FirstOrDefault(e => e.Key == "intent").Value?
                               .OrderByDescending(i => i.Confidence)
                               .Select(i => i.Value)
                               .FirstOrDefault();

            if (string.IsNullOrEmpty(result.Text) || string.IsNullOrEmpty(intent) || !this.handlerByIntent.TryGetValue(intent, out IntentActivityHandler handler))
            {
                handler = this.handlerByIntent[string.Empty];
            }

            if (handler != null)
            {
                await handler(context, item, result);
            }
            else
            {
                throw new ActionHandlerNotFoundException("No default intent handler found.");
            }
        }

        protected virtual IDictionary<string, IntentActivityHandler> GetHandlersByIntent()
        {
            return WitDialog.EnumerateHandlers(this).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        protected virtual Task<string> GetWitQueryTextAsync(IDialogContext context, IMessageActivity message)
        {
            return Task.FromResult(message.Text);
        }
    }

    internal static class WitDialog
    {
        public static IEnumerable<KeyValuePair<string, IntentActivityHandler>> EnumerateHandlers(object dialog)
        {
            var type = dialog.GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var method in methods)
            {
                var actions = method.GetCustomAttributes<WitIntentAttribute>(inherit: true).ToArray();
                IntentActivityHandler intentHandler = null;

                try
                {
                    intentHandler = (IntentActivityHandler)Delegate.CreateDelegate(typeof(IntentActivityHandler), dialog, method, throwOnBindFailure: false);
                }
                catch (ArgumentException)
                {
                    // "Cannot bind to the target method because its signature or security transparency is not compatible with that of the delegate type."
                    // https://github.com/Microsoft/BotBuilder/issues/634
                    // https://github.com/Microsoft/BotBuilder/issues/435
                }

                // fall back for compatibility
                if (intentHandler == null)
                {
                    try
                    {
                        var handler = (IntentHandler)Delegate.CreateDelegate(typeof(IntentHandler), dialog, method, throwOnBindFailure: false);

                        if (handler != null)
                        {
                            // thunk from new to old delegate type
                            intentHandler = (context, message, result) => handler(context, result);
                        }
                    }
                    catch (ArgumentException)
                    {
                        // "Cannot bind to the target method because its signature or security transparency is not compatible with that of the delegate type."
                        // https://github.com/Microsoft/BotBuilder/issues/634
                        // https://github.com/Microsoft/BotBuilder/issues/435
                    }
                }

                if (intentHandler != null)
                {
                    var intentNames = actions.Select(i => i.IntentName).DefaultIfEmpty(method.Name);

                    foreach (var intentName in intentNames)
                    {
                        var key = string.IsNullOrWhiteSpace(intentName) ? string.Empty : intentName;
                        yield return new KeyValuePair<string, IntentActivityHandler>(intentName, intentHandler);
                    }
                }
                else
                {
                    if (actions.Length > 0)
                    {
                        throw new InvalidActionHandlerException(string.Join(";", actions.Select(i => i.IntentName)), method);
                    }
                }
            }
        }
    }
}
