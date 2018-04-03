using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Framework.Builder.Witai.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Bot.Framework.Builder.Witai
{
    [Serializable]
    public sealed class WitService : IWitService
    {
        private readonly IWitModel _model;
        private readonly IWitModelProvider _modelProvider;

        /// <summary>
        /// Construct the wit service using the model information.
        /// </summary>
        /// <param name="model">The wit model information.</param>
        public WitService(IWitModel model)
        {
            SetField.NotNull(out _model, nameof(model), model);
        }

        public WitService(IWitModelProvider modelProvider)
        {
            SetField.NotNull(out _modelProvider, nameof(modelProvider), modelProvider);
        }

        public async Task<WitResult> QueryAsync(IDialogContext context, IWitRequest request, CancellationToken token)
        {
            IWitModel model = _model;
            if (_modelProvider != null)
            {
                model = await _modelProvider.GetWitModelAsync(context);
            }

            var httpRequest = BuildRequest(request, model);
            return await QueryAsync(httpRequest, token);
        }

        public HttpRequestMessage BuildRequest(IWitRequest witRequest, IWitModel model)
        {
            return witRequest.BuildRequest(model);
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

