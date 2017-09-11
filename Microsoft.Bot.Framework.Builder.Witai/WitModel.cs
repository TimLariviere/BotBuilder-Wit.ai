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
        private readonly string _authToken;
        private readonly Uri _uriBase;
        private readonly WitApiVersion _apiVersion;

        /// <summary>
        /// Construct the Wit model information.
        /// </summary>
        /// <param name="authToken">The Wit model authorization token.</param>
        /// <param name="apiVersionType">The wit API version (Latest or Custom).</param>
        /// <param name="apiVersion">The wit API version.</param>
        public WitModelAttribute(string authToken, WitApiVersionType apiVersionType = WitApiVersionType.Latest, string apiVersion = null)
        {
            _apiVersion = apiVersionType == WitApiVersionType.Latest ? WitApiVersion.Latest : WitApiVersion.Custom(apiVersion);

            SetField.NotNull(out _authToken, nameof(authToken), authToken);
            SetField.NotNull(out _uriBase, nameof(_uriBase), BuildUri());
        }

        public string AuthToken => _authToken;
        public Uri UriBase => _uriBase;
        public WitApiVersion ApiVersion { get; }

        private Uri BuildUri()
        {
            string url = null;
            switch (_apiVersion.Type)
            {
                case WitApiVersionType.Latest:
                    url = "https://api.wit.ai/message";
                    break;
                case WitApiVersionType.Custom:
                    url = "https://api.wit.ai/message?v=" + _apiVersion.CustomValue;
                    break;
                default:
                    throw new ArgumentException($"This is not a valid Wit api version.");
            }

            return new Uri(url);
        }
    }
}
