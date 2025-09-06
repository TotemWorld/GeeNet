using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IntegrationTests.TokenProvider
{
    public class TokenProviderIntegrationTests
    {
        [Fact]
        public async Task GetTokenAsync_ShouldReturnCachedToken_WhenCalled()
        {
            var options = new GeeNet.GeeNetOptions
            {
                GoogleServiceAccount = "service-account.json",
                Scope = "https://www.googleapis.com/auth/earthengine",
            };

            var serviceAccountFile = options.GoogleServiceAccount;
            var jsonText = File.ReadAllText(serviceAccountFile);
            using var doc = JsonDocument.Parse(jsonText);
            var root = doc.RootElement;

            string projectId = root.GetProperty("project_id").GetString() ?? throw new Exception("project_id not found in service account json file");
            options.ProjectId = projectId;

            var iOptions = Options.Create(options);

            var tokenProvider = new GeeNet.TokenProvider(iOptions);

            var token = await tokenProvider.GetTokenAsync();

            var token2 = await tokenProvider.GetTokenAsync();

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var content = """
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
                                        "coordinates": { "constantValue": [102.0, 0.5] }
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

            var httpContent = new StringContent(content, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"https://earthengine.googleapis.com/v1/projects/{options.ProjectId}/imageCollection:computeImages", httpContent);

            Assert.NotNull(token);
            Assert.Equal(token, token2);    
        }
    }
}
