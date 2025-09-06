using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using GeeNet.Datasets;
using GeeNet.GeoTypes;

namespace GeeNet
{
    internal class ImageCollectionExpressionWrapper
    {
        [JsonPropertyName("expression")]
        public required Expression Expression { get; set; }
    }
    
    internal class ImageExpressionWrapper
    {
        [JsonPropertyName("expression")]
        public required Expression Expression { get; set; }
        [JsonPropertyName("fileFormat")]
        public required string FileFormat { get; set; }
        [JsonPropertyName("bandIds")]
        public string[]? BandIds { get; set; }
        [JsonPropertyName("grid")]
        public required PixelGrid Grid { get; set; }

        public static JsonSerializerOptions options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    internal class Expression
    {
        [JsonPropertyName("result")]
        public required string Result { get; set; }
        [JsonPropertyName("values")]
        public required Dictionary<string, object> Values { get; set; }
    }

    internal class FunctionInvocation
    {
        [JsonPropertyName("functionInvocationValue")]
        public required FunctionInvocationValue FunctionInvocationValue { get; set; }
    }

    internal class FunctionInvocationValue
    {
        [JsonPropertyName("functionName")]
        public required string FunctionName { get; set; }
        [JsonPropertyName("arguments")]
        public required Dictionary<string, object> Arguments { get; set; }
    }

    internal class ConstantValue
    {
        [JsonPropertyName("constantValue")]
        public required object ConstantValue_ { get; set; }
    }

    internal class ExpressionBuilder
    {
        private object _currentExpression;

        public ImageExpressionWrapper BuildImageExpression(PixelGrid pixelGrid, string[]? bandIds = null)
        {
            var expressionWrapper = new ImageExpressionWrapper
            {
                Expression = new Expression
                {
                    Result = "0",
                    Values = new Dictionary<string, object>
                    {
                        ["0"] = _currentExpression
                    }
                },
                FileFormat = "GEO_TIFF",
                //BandIds = bandIds,
                Grid = pixelGrid
            };

            return expressionWrapper;
        }
        
        public ImageCollectionExpressionWrapper BuildImageCollectionExpression()
        {
            var expressionWrapper = new ImageCollectionExpressionWrapper
            {
                Expression = new Expression
                {
                    Result = "0",
                    Values = new Dictionary<string, object>
                    {
                        ["0"] = _currentExpression
                    }
                }
            };

            return expressionWrapper;
        }

        public static ExpressionBuilder LoadImageCollection(string id)
        {
            var expressionBuilder = new ExpressionBuilder();
            expressionBuilder._currentExpression = new FunctionInvocation()
            {
                FunctionInvocationValue = new FunctionInvocationValue
                {
                    FunctionName = "ImageCollection.load",
                    Arguments = new Dictionary<string, object>
                    {
                        { "id", new ConstantValue { ConstantValue_ = id } }
                    }

                }
            };

            return expressionBuilder;
        }

        public  ExpressionBuilder FilterByDateRange(string startDate, string endDate, string timeField = "system:time_start")
        {
            _currentExpression = new FunctionInvocation
            {
                FunctionInvocationValue = new FunctionInvocationValue 
                { 
                    FunctionName = "Collection.filter",
                    Arguments = new Dictionary<string, object>
                    {
                        ["collection"] = _currentExpression,
                        ["filter"] = new FunctionInvocation
                        {
                            FunctionInvocationValue = new FunctionInvocationValue
                            {
                                FunctionName = "Filter.dateRangeContains",
                                Arguments = new Dictionary<string, object>
                                {
                                    ["leftValue"] = new FunctionInvocation
                                    {
                                        FunctionInvocationValue = new FunctionInvocationValue
                                        {
                                            FunctionName = "DateRange",
                                            Arguments = new Dictionary<string, object>
                                            {
                                                ["start"] = new ConstantValue { ConstantValue_ = startDate },
                                                ["end"] = new ConstantValue { ConstantValue_ = endDate }
                                            }
                                        }
                                    },
                                    ["rightField"] = new ConstantValue { ConstantValue_ = timeField }
                                }
                            }
                        }
                    }
                }
            };
            return this;
        }

        public ExpressionBuilder FilterByGeometry(IGeoType geoType)
        {
            FunctionInvocation functionInvocation;

            if (geoType is Point)
            {
                functionInvocation = new FunctionInvocation
                {
                    FunctionInvocationValue = new FunctionInvocationValue
                    {
                        FunctionName = "GeometryConstructors.Point",
                        Arguments = new Dictionary<string, object>
                        {
                            ["coordinates"] = new ConstantValue
                            {
                                ConstantValue_ = new double[] { ((Point)geoType).X, ((Point)geoType).Y }
                            }
                        }
                    }
                };
            }
            else if (geoType is Bounds)
            {
                functionInvocation = new FunctionInvocation
                {
                    FunctionInvocationValue = new FunctionInvocationValue
                    {
                        FunctionName = "GeometryConstructors.Rectangle",
                        Arguments = new Dictionary<string, object>
                        {
                            ["coordinates"] = new ConstantValue
                            {
                                ConstantValue_ = new double[][]
                                {
                                    [((Bounds)geoType).Min.X, ((Bounds)geoType).Min.Y],
                                    [((Bounds)geoType).Max.X, ((Bounds)geoType).Max.Y]
                                }
                            }
                        }
                    }
                };
            }
            else
            {
                throw new ArgumentException("geoType is not Point or Bounds");
            }

                _currentExpression = new FunctionInvocation
                {
                    FunctionInvocationValue = new FunctionInvocationValue
                    {
                        FunctionName = "Collection.filter",
                        Arguments = new Dictionary<string, object>
                        {
                            ["collection"] = _currentExpression,
                            ["filter"] = new FunctionInvocation
                            {
                                FunctionInvocationValue = new FunctionInvocationValue
                                {
                                    FunctionName = "Filter.intersects",
                                    Arguments = new Dictionary<string, object>
                                    {
                                        ["leftField"] = new ConstantValue { ConstantValue_ = ".geo" },
                                        ["rightValue"] = functionInvocation
                                    }
                                }
                            }
                        }
                    }
                };
            return this;
        }

        public ExpressionBuilder FilterByCloudCover(double cloudCover)
        {
            _currentExpression = new FunctionInvocation
            {
                FunctionInvocationValue = new FunctionInvocationValue
                {
                    FunctionName = "Collection.filter",
                    Arguments = new Dictionary<string, object>
                    {
                        ["collection"] = _currentExpression,
                        ["filter"] = new FunctionInvocation
                        {
                            FunctionInvocationValue = new FunctionInvocationValue
                            {
                                FunctionName = "Filter.lt",
                                Arguments = new Dictionary<string, object>
                                {
                                    ["leftField"] = new ConstantValue { ConstantValue_ = "CLOUD_COVER" },
                                    ["rightValue"] = new ConstantValue { ConstantValue_ = cloudCover }
                                }
                            }
                        }
                    }
                }
            };

            return this;
        }
    
        public ExpressionBuilder ReduceWith(string reducerFunction)
        {
            _currentExpression = new FunctionInvocation
            {
                FunctionInvocationValue = new FunctionInvocationValue
                {
                    FunctionName = "ImageCollection.reduce",
                    Arguments = new Dictionary<string, object>
                    {
                        ["collection"] = _currentExpression,
                        ["reducer"] = new FunctionInvocation
                        {
                            FunctionInvocationValue = new FunctionInvocationValue
                            {
                                FunctionName = $"Reducer.{reducerFunction}",
                                Arguments = new Dictionary<string, object>()
                            }
                        }
                    }
                }
            };

            return this;
        }
    
        public ExpressionBuilder ClipImage(Bounds bounds)
        {
            _currentExpression = new FunctionInvocation
            {
                FunctionInvocationValue = new FunctionInvocationValue
                {
                    FunctionName = "Image.clip",
                    Arguments = new Dictionary<string, object>
                    {
                        ["image"] = _currentExpression,
                        ["geometry"] = new FunctionInvocation
                        {
                            FunctionInvocationValue = new FunctionInvocationValue
                            {
                                FunctionName = "GeometryConstructors.Rectangle",
                                Arguments = new Dictionary<string, object>
                                {
                                    ["coordinates"] = new ConstantValue
                                    {
                                        ConstantValue_ = new double[][]
                                        {
                                            [bounds.Min.X, bounds.Min.Y],
                                            [bounds.Max.X, bounds.Max.Y]
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            return this;
        }
       
        public ExpressionBuilder SelectBands(string[] bands)
        {
            _currentExpression = new FunctionInvocation
            {
                FunctionInvocationValue = new FunctionInvocationValue
                {
                    FunctionName = "Image.select",
                    Arguments = new Dictionary<string, object>
                    {
                        ["input"] = _currentExpression,
                        ["bandSelectors"] = new ConstantValue { ConstantValue_ = bands }
                    }
                }
            };
            return this;
        }
    }
}