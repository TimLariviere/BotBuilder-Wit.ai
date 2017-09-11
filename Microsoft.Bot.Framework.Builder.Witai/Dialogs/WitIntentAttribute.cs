using System;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables.Internals;

namespace Microsoft.Bot.Framework.Builder.Witai.Dialogs
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    [Serializable]
    public class WitIntentAttribute : AttributeString
    {
        private readonly string _intentName;

        public WitIntentAttribute(string intentName)
        {
            SetField.NotNull(out _intentName, nameof(intentName), intentName);
        }

        protected override string Text => _intentName;
        public string IntentName => _intentName;
    }
}
