using Microsoft.Bot.Builder.Internals.Fibers;
using System;

namespace Microsoft.Bot.Framework.Builder.Witai
{
    /// <summary>
    /// The Wit model information.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
    [Serializable]
    public class WitModelAttribute : Attribute, IWitModel
    {
        private readonly string authToken;

        public string AuthToken => authToken;

        private readonly Uri uriBase;

        public Uri UriBase => uriBase;

        private readonly WitApiVersion apiVersion;

        public WitApiVersion ApiVersion => apiVersion;

        /// <summary>
        /// Construct the Wit model information.
        /// </summary>
        /// <param name="authToken">The Wit model authorization token.</param>
        /// <param name="apiVersion">The wit API version.</param>
        public WitModelAttribute(string authToken, WitApiVersion apiVersion = null)
        {
            apiVersion = apiVersion ?? WitApiVersion.Latest;

            SetField.NotNull(out this.authToken, nameof(authToken), authToken);
            this.apiVersion = apiVersion;
            SetField.NotNull(out this.uriBase, nameof(uriBase), this.BuildUri());
        }

        private Uri BuildUri()
        {
            string url = null;
            switch (this.apiVersion.Type)
            {
                case WitApiVersionType.Latest:
                    url = "https://api.wit.ai/message";
                    break;
                case WitApiVersionType.Custom:
                    url = "https://api.wit.ai/message?v=" + this.apiVersion.CustomValue;
                    break;
                default:
                    throw new ArgumentException($"This is not a valid Wit api version.");
            }

            return new Uri(url);
        }
    }
}
