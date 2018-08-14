using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Framework.Builder.Exceptions;
using Microsoft.Bot.Framework.Builder.Witai.Extensions;
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
        #region Fields

        [NonSerialized]
        private Dictionary<string, IntentActivityHandler> _handlerByIntent;
        private readonly IWitService _service;
        private readonly WitConfiguration _configuration;
        private string _witThreadId;

        #endregion

        #region Constructor

        public WitDialog()
        {
            SetField.NotNull(out _service, nameof(_service), MakeServiceFromAttributes());
            StartNewThread();
        }

        public WitDialog(IWitService service, WitConfiguration configuration)
        {
            SetField.NotNull(out _service, nameof(service), service);
            SetField.NotNull(out _configuration, nameof(configuration), configuration);
            StartNewThread();
        }

        #endregion

        #region Properties

        protected IWitService Service => _service;
        protected IWitContext Context { get; set; }

        #endregion

        #region Public methods

        public virtual Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceived);
            return Task.CompletedTask;
        }

        #endregion

        #region Protected methods

        protected void StartNewThread()
        {
            Context = new WitContext();
            _witThreadId = Guid.NewGuid().ToString();
        }

        protected virtual async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            await MessageHandler(context, item);
        }

        protected virtual async Task DispatchToIntentHandler(IDialogContext context, IAwaitable<IMessageActivity> item, WitResult result)
        {
            if (_handlerByIntent == null)
            {
                _handlerByIntent = new Dictionary<string, IntentActivityHandler>(GetHandlersByIntent());
            }

            var intent = result.Entities?.FirstOrDefault(e => e.Key == "intent").Value?
                               .OrderByDescending(i => i.Confidence)
                               .Where(i => i.Confidence >= _configuration.MinConfidenceThreshold)
                               .Select(i => i.Value)
                               .FirstOrDefault();

            if (string.IsNullOrEmpty(result.Text) || string.IsNullOrEmpty(intent) || !_handlerByIntent.TryGetValue(intent, out IntentActivityHandler handler))
            {
                handler = _handlerByIntent[string.Empty];
            }

            if (handler != null)
            {
                await handler(context, item, result);
            }
            else
            {
                throw new IntentHandlerNotFoundException("No default intent handler found.");
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

        #endregion

        #region Private methods

        private async Task MessageHandler(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var message = await item;
            var messageText = await GetWitQueryTextAsync(context, message);
            var jsonContext = Context.ToJsonString();
            var result = await _service.QueryAsync(context, messageText, _witThreadId, jsonContext, context.CancellationToken);

            await DispatchToIntentHandler(context, item, result);
        }

        private IWitService MakeServiceFromAttributes()
        {
            var type = GetType();
            var witModels = type.GetCustomAttributes<WitModelAttribute>(inherit: true);
            if (witModels.ToArray().Length > 1)
            {
                throw new WitModelDisambiguationException("WitDialog does not support more than one WitModel per instance");
            }

            var attribute = witModels.ToArray()[0];
            var witModel = attribute.MakeWitModel();

            return new WitService(witModel);
        }

        #endregion
    }

    internal static class WitDialog
    {
        public static IEnumerable<KeyValuePair<string, IntentActivityHandler>> EnumerateHandlers(object dialog)
        {
            var type = dialog.GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var method in methods)
            {
                var actions = method.GetCustomAttributes<WitIntentAttribute>(inherit: true).ToArray();
                IntentActivityHandler intentHandler = null;

                try
                {
                    if (method.IsStatic)
                    {
                        intentHandler = (IntentActivityHandler)Delegate.CreateDelegate(typeof(IntentActivityHandler), method, throwOnBindFailure: false);
                    }
                    else
                    {
                        intentHandler = (IntentActivityHandler)Delegate.CreateDelegate(typeof(IntentActivityHandler), dialog, method, throwOnBindFailure: false);
                    }                    
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
                        IntentHandler handler = null;

                        if (method.IsStatic)
                        {
                            handler = (IntentHandler)Delegate.CreateDelegate(typeof(IntentHandler), method, throwOnBindFailure: false);
                        }
                        else
                        {
                            handler = (IntentHandler)Delegate.CreateDelegate(typeof(IntentHandler), dialog, method, throwOnBindFailure: false);
                        }

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
                        throw new InvalidIntentHandlerException(string.Join(";", actions.Select(i => i.IntentName)), method);
                    }
                }
            }
        }
    }
}
