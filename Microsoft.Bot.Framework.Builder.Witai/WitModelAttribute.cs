using Microsoft.Bot.Builder.Internals.Fibers;
using System;

namespace Microsoft.Bot.Framework.Builder.Witai
{
    /// <summary>
    /// The Wit model information.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
    [Serializable]
    public class WitModelAttribute : Attribute
    {
        private readonly string _authToken;
        private readonly WitApiVersionType _apiVersionType;
        private readonly string _apiVersion;

        /// <summary>
        /// Construct the Wit model information.
        /// </summary>
        /// <param name="authToken">The Wit model authorization token.</param>
        /// <param name="apiVersionType">The wit API version (Latest or Custom).</param>
        /// <param name="apiVersion">The wit API version.</param>
        public WitModelAttribute(string authToken, WitApiVersionType apiVersionType = WitApiVersionType.Latest, string apiVersion = null)
        {
            SetField.NotNull(out _authToken, nameof(authToken), authToken);
            _apiVersionType = apiVersionType;
            _apiVersion = apiVersion;
        }

        public string AuthToken => _authToken;
        public WitApiVersionType ApiVersionType => _apiVersionType;
        public string ApiVersion => _apiVersion;

        public WitModel MakeWitModel()
        {
            return new WitModel(_authToken, _apiVersionType, _apiVersion);
        }
    }
}
