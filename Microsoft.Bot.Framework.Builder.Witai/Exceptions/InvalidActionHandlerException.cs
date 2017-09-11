using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.Bot.Framework.Builder.Witai.Dialogs
{
    [Serializable]
    internal class InvalidIntentHandlerException : Exception
    {
        private MethodInfo method;
        private string v;

        public InvalidIntentHandlerException()
        {
        }

        public InvalidIntentHandlerException(string message) : base(message)
        {
        }

        public InvalidIntentHandlerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidIntentHandlerException(string v, MethodInfo method)
        {
            this.v = v;
            this.method = method;
        }

        protected InvalidIntentHandlerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}