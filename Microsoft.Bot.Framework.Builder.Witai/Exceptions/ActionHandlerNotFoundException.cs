using System;

namespace Microsoft.Bot.Framework.Builder.Exceptions
{
    /// <summary>
    /// Exception type thrown when an ActionHandler is not found
    /// </summary>
    [Serializable]
    internal class IntentHandlerNotFoundException : Exception
    {
        public IntentHandlerNotFoundException(string message) : base(message)
        {
        }
    }
}
