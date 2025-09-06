using GeeNet.Constants;
using GeeNet.GeoTypes;
using GeeNet.Helpers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Google.Apis.Requests.BatchRequest;

namespace GeeNet.Ee
{
    public class ImagePixelsBuilder
    {
        private const string _endpoint = "image:computePixels";
        private readonly string? _id;
        private  ExpressionBuilder? _expressionBuilder;
        private readonly GeeNetClient _client;
        private readonly GeeNetOptions _options;
        private string[]? _bands;

        private PixelGrid? _grid;

        internal ImagePixelsBuilder(string id, GeeNetClient client, IOptions<GeeNetOptions> options)
        {
            _id = id;
            _client = client;
            _options = options.Value ?? throw new ArgumentNullException(nameof(options), "GeeNetOptions cannot be null.");
        }

        internal ImagePixelsBuilder(ExpressionBuilder expression, GeeNetClient client, IOptions<GeeNetOptions> options)
        {
            _expressionBuilder = expression;
            _client = client;
            _options = options.Value ?? throw new ArgumentNullException(nameof(options), "GeeNetOptions cannot be null.");
        }

        public ImagePixelsBuilder SetPixelsGrid(Bounds subset, double gridDimension)
        {
            var crs = UTMHelper.GetUtmCrsCode(subset.MiddlePoint.X);
            var utmBounds = UTMHelper.ConvertToUtm(subset);

            var width = utmBounds.Max.X - utmBounds.Min.X;
            var height = utmBounds.Max.Y - utmBounds.Min.Y;

            var pixelHeight = (int)Math.Ceiling(height / gridDimension);
            var pixelWidth = (int)Math.Ceiling(width / gridDimension);


            _grid = new PixelGrid
            {
                Dimensions = new Dimensions
                {
                    Width = pixelWidth,
                    Height = pixelHeight
                },
                AffineTransform = new AffineTransform
                {
                    ScaleX = gridDimension,
                    ShearX = 0,
                    TranslateX = utmBounds.Min.X,
                    ScaleY = -gridDimension,
                    ShearY = 0,
                    TranslateY = utmBounds.Max.Y,
                },
                CrsCode = crs,
            };

            var json = JsonSerializer.Serialize(_grid, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
            Console.WriteLine(json);

            return this;

        }

        public ImagePixelsBuilder SelectBands(params string[] bands)
        {
            if (_expressionBuilder == null) throw new InvalidOperationException("ExpressionBuilder is null. Cannot select bands.");
            _expressionBuilder = _expressionBuilder.SelectBands(bands);
            return this;
        }

        public ImagePixelsBuilder SetPixelsGrid(PixelGrid pixelGrid)
        {
            _grid = pixelGrid;
            return this;
        }

        public ImagePixelsBuilder SetBands(string[] bands)
        {
            _bands = bands;
            return this;
        }

        public async Task<Stream> BuildAndFetchAsync()
        {
            if(_expressionBuilder == null) throw new InvalidOperationException("ExpressionBuilder is null. Cannot build image expression.");
            if(_grid == null) throw new InvalidOperationException("PixelGrid is null. Please set the pixel grid using SetPixelsGrid method before fetching image pixels.");
            var expression = _expressionBuilder.BuildImageExpression(_grid, _bands);

            var json = JsonSerializer.Serialize(expression, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            var completeEndpoint = $"projects/{_options.ProjectId}/{_endpoint}";

            var result = await _client.PostAsync(completeEndpoint, json);

            var contentType = result.Content.Headers.ContentType?.MediaType;

            var responseStream = await result.Content.ReadAsStreamAsync();

            return responseStream;
        }
    }
}
