using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Microsoft.Bot.Framework.Builder.Witai
{
    /// <summary>
    /// Object that contains all the possible parameters to build a Wit request.
    /// </summary>
    public sealed class WitRequest : IWitRequest
    {
        public string Query { get; }

        public string ThreadId => threadId;

        private string threadId;

        public string Context => context;

        private string context;

        public WitRequest(string query, string threadId, string context = "{}")
        {
            this.Query = query;
            SetField.NotNull(out this.threadId, nameof(threadId), threadId);
            SetField.NotNull(out this.context, nameof(context), context);
        }

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
                RequestUri = this.BuildUri(model),
                Method = HttpMethod.Post,
            };

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", model.AuthToken);
            request.Content = new StringContent(this.Context, Encoding.UTF8, "application/json");

            return request;
        }

        
        private Uri BuildUri(IWitModel model)
        {
            if (ThreadId == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "thread id");
            }

            var queryParameters = new List<string>();
            queryParameters.Add($"thread_id={Uri.EscapeDataString(ThreadId)}");

            if (!string.IsNullOrEmpty(Query))
            {
                queryParameters.Add($"q={Uri.EscapeDataString(Query)}");
            }

            var builder = new UriBuilder(model.UriBase)
            {
                Query = string.Join("&", queryParameters)
            };
            return builder.Uri;
        }
    }

}
