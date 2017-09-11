using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Framework.Builder.Witai.Models;
using Newtonsoft.Json;

namespace Microsoft.Bot.Framework.Builder.Witai
{
    [Serializable]
    public sealed class WitService : IWitService
    {
        private readonly IWitModel _model;

        /// <summary>
        /// Construct the wit service using the model information.
        /// </summary>
        /// <param name="model">The wit model information.</param>
        public WitService(IWitModel model)
        {
            SetField.NotNull(out _model, nameof(model), model);
        }

        public Task<WitResult> QueryAsync(IWitRequest request, CancellationToken token)
        {
            var httpRequest = BuildRequest(request);
            return QueryAsync(httpRequest, token);
        }

        public HttpRequestMessage BuildRequest(IWitRequest witRequest)
        {
            return witRequest.BuildRequest(_model);
        }

        private async Task<WitResult> QueryAsync(HttpRequestMessage request, CancellationToken token)
        {
            var json = string.Empty;

            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();
            }

            try
            {
                return JsonConvert.DeserializeObject<WitResult>(json);
            }
            catch (JsonException ex)
            {
                throw new ArgumentException("Unable to deserialize the Wit response.", ex);
            }
        }
    }
}

