using GeeNet.GeoTypes;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GeeNet.Ee
{

    public class ImageCollectionBuilder
    {
        private string _id;
        private readonly GeeNetClient _client;
        private const string _endpoint = "imageCollection:computeImages";
        private ExpressionBuilder _expressionBuilder;
        private GeeNetOptions _options;
        internal ImageCollectionBuilder(string id, GeeNetClient client, IOptions<GeeNetOptions> options) 
        {
            _client = client;
            _id = id;
            _expressionBuilder = ExpressionBuilder.LoadImageCollection(_id);
            _options = options.Value ?? throw new ArgumentNullException(nameof(options), "GeeNetOptions cannot be null.");
        }

        public ImageCollectionBuilder FilterByDateRange(string startDate, string endDate)
        {
            _expressionBuilder = _expressionBuilder.FilterByDateRange(startDate, endDate);
            return this;
        }

        public ImageCollectionBuilder FilterByGeometry(IGeoType geometry)
        {
            _expressionBuilder = _expressionBuilder.FilterByGeometry(geometry);
            return this;
        }

        public ImageCollectionBuilder FilterByCloudCover(double cloudCover)
        {
            _expressionBuilder = _expressionBuilder.FilterByCloudCover(cloudCover);
            return this;
        }


        public ImagePixelsBuilder ReduceWith(string function)
        {
            _expressionBuilder = _expressionBuilder.ReduceWith(function);
            return new ImagePixelsBuilder(_expressionBuilder, _client, Options.Create(_options));
        }

        public async Task<List<ImageMetadata>> BuildAndFetchAsync()
        {
            var expression = _expressionBuilder.BuildImageCollectionExpression();
            var json = JsonSerializer.Serialize(expression, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            var completeEndpoint = $"projects/{_options.ProjectId}/{_endpoint}";

            var result = await _client.PostAsync(completeEndpoint, json);

            var content = await result.Content.ReadAsStringAsync();

            var images = ImageMetadata.GetImages(JsonDocument.Parse(content));
            return images;
        }
    }
}
