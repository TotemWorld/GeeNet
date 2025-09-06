using GeeNet;
using GeeNet.Datasets;
using GeeNet.GeoTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace UnitTests
{
    public class ExpressionBuilder_UnitTests
    {
        [Fact]
        public void LoadImageCollection_And_FilterByRange_And_FilterByGeo_ReturnsNestedExpression()
        {
            var jsonTest = """
                {
                  "expression": {
                    "result": "0",
                    "values": {
                      "0": {
                        "functionInvocationValue": {
                          "functionName": "Collection.filter",
                          "arguments": {
                            "collection": {
                              "functionInvocationValue": {
                                "functionName": "Collection.filter",
                                "arguments": {
                                  "collection": {
                                    "functionInvocationValue": {
                                      "functionName": "ImageCollection.load",
                                      "arguments": {
                                        "id": { "constantValue": "LANDSAT/LC08/C02/T2_L2" }
                                      }
                                    }
                                  },
                                  "filter": {
                                    "functionInvocationValue": {
                                      "functionName": "Filter.dateRangeContains",
                                      "arguments": {
                                        "leftValue": {
                                          "functionInvocationValue": {
                                            "functionName": "DateRange",
                                            "arguments": {
                                              "start": { "constantValue": "2022-01-01" },
                                              "end": { "constantValue": "2022-01-31" }
                                            }
                                          }
                                        },
                                        "rightField": { "constantValue": "system:time_start" }
                                      }
                                    }
                                  }
                                }
                              }
                            },
                            "filter": {
                              "functionInvocationValue": {
                                "functionName": "Filter.intersects",
                                "arguments": {
                                  "leftField": { "constantValue": ".geo" },
                                  "rightValue": {
                                    "functionInvocationValue": {
                                      "functionName": "GeometryConstructors.Point",
                                      "arguments": {
                                        "coordinates": { "constantValue": [
                                            102, 
                                            0.5
                                            ] 
                                        }
                                      }
                                    }
                                  }
                                }
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                }
                """;
            var expression = ExpressionBuilder.LoadImageCollection(Landsat.LC08.C02.T2_L2.Value())
                .FilterByDateRange("2022-01-01", "2022-01-31")
                .FilterByGeometry(new Point { X = 102.0, Y = 0.5 })
                .BuildImageCollectionExpression();

            var json = System.Text.Json.JsonSerializer.Serialize(expression, new JsonSerializerOptions { WriteIndented = true });

            JObject jObject1 = JObject.Parse(jsonTest);
            JObject jObject2 = JObject.Parse(json)!;

            Assert.True(JObject.DeepEquals(jObject1, jObject2));

        }
    }
}
