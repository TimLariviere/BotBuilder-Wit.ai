using System;

namespace Microsoft.Bot.Framework.Builder.Exceptions
{
    /// <summary>
    /// Exception type thrown when an unsupported action is received from Wit Api
    /// </summary>
    [Serializable]
    internal class UnsupportedWitIntentException : Exception
    {
        public UnsupportedWitIntentException(string message) : base(message)
        {
        }
    }
}
