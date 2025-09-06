using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GeeNet.GeoTypes
{
//    const gridConfig = {
//  "dimensions": {
//    "width": gridWidth,
//    "height": gridHeight
//  },
//  "affineTransform": {
//    "scaleX": pixelSizeLon,      // Pixel size in longitude degrees
//    "shearX": 0.0,               // No rotation
//    "translateX": west,          // Western boundary (left edge)
//    "scaleY": -pixelSizeLat,     // Negative for north-up orientation
//    "shearY": 0.0,               // No rotation  
//    "translateY": north,         // Northern boundary (top edge)
//  },
//  "crsCode": "EPSG:4326"
//};
    public class PixelGrid
    {
        public PixelGrid() { }

        [JsonPropertyName("dimensions")]
        public Dimensions Dimensions { get; set; }
        [JsonPropertyName("affineTransform")]
        public AffineTransform AffineTransform { get; set; }
        [JsonPropertyName("crsCode")]
        public string CrsCode { get; set; }
    }

    public class  Dimensions
    {
        [JsonPropertyName("width")]
        public double Width { get; set; }
        [JsonPropertyName("height")]
        public double Height { get; set; }
    }

    public class AffineTransform
    {
        [JsonPropertyName("scaleX")]
        public double ScaleX { get; set; }
        [JsonPropertyName("shearX")]
        public double ShearX { get; set; }
        [JsonPropertyName("translateX")]
        public double TranslateX { get; set; }
        [JsonPropertyName("scaleY")]
        public double ScaleY { get; set; }
        [JsonPropertyName("shearY")]
        public double ShearY { get; set; }
        [JsonPropertyName("translateY")]
        public double TranslateY { get; set; }
    }
}
