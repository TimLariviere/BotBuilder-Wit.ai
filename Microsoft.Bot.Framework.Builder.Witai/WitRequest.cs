using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.Bot.Framework.Builder.Witai
{
    /// <summary>
    /// Object that contains all the possible parameters to build a Wit request.
    /// </summary>
    public sealed class WitRequest : IWitRequest
    {
        private string _threadId;

        public WitRequest(string query, string threadId, string context = null)
        {
            Query = query;
            Context = context;
            SetField.NotNull(out _threadId, nameof(threadId), threadId);
        }

        public string Query { get; }
        public string Context { get; }
        public string ThreadId => _threadId;

        /// <summary>
        /// Build the Uri for issuing the request for the specified wit model.
        /// </summary>
        /// <param name="model"> The wit model.</param>
        /// <returns> The request Uri.</returns>
        public HttpRequestMessage BuildRequest(IWitModel model)
        {
            if (model.AuthToken == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Authorization Token");
            }

            var request = new HttpRequestMessage()
            {
                RequestUri = BuildUri(model),
                Method = HttpMethod.Get,
            };

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", model.AuthToken);

            return request;
        }

        
        private Uri BuildUri(IWitModel model)
        {
            if (ThreadId == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "thread id");
            }

            var queryParameters = new List<string>
            {
                $"thread_id={Uri.EscapeDataString(ThreadId)}"
            };

            if (!string.IsNullOrEmpty(Query))
            {
                queryParameters.Add($"q={Uri.EscapeDataString(Query)}");
            }

            if (!string.IsNullOrEmpty(Context))
            {
                queryParameters.Add($"context={Uri.EscapeDataString(Context)}");
            }

            var builder = new UriBuilder(model.UriBase)
            {
                Query = string.Join("&", queryParameters)
            };


            return builder.Uri;
        }
    }

}
