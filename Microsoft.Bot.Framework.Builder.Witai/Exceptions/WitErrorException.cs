using System;

namespace Microsoft.Bot.Framework.Builder.Witai.Exceptions
{
    [Serializable]
    internal class WitErrorException : Exception
    {
        public WitErrorException()
        {
        }

        public WitErrorException(string message) : base(message)
        {
        }
    }
}