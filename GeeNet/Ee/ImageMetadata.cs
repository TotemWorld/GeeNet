using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GeeNet.Ee
{
    public class ImageMetadata
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("properties")]
        public required ImageProperties Properties { get; set; }

        public static List<ImageMetadata> GetImages(JsonDocument json)
        {
            var imagesList = new List<ImageMetadata>();
            if(json.RootElement.TryGetProperty("images", out JsonElement imagesElement))
            {
                foreach (var image in imagesElement.EnumerateArray())
                {
                    var imgObj = JsonSerializer.Deserialize<ImageMetadata>(image) ?? throw new JsonException("Failed to deserialize image object.");
                    imagesList.Add(imgObj);
                }
            }
            return imagesList;

        }
    }

    public class ImageProperties
    {
        [JsonPropertyName("CLOUD_COVER")]
        public double CloudCover { get; set; }
    }
}
