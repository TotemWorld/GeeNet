using GeeNet;
using GeeNet.Datasets;
using GeeNet.Ee;
using GeeNet.GeoTypes;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace IntegrationTests;

public class ImageCollectionBuilderTests
{
    [Fact]
    public async Task ImageCollectionBuilder_FullWorkflow_RetrieveImageMetadataList()
    {
        #region Arrange

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

        var client = new GeeNetClient(iOptions, new HttpClient(), tokenProvider);

        #endregion

        #region Act

        var ee = new Ee(client, iOptions);

        var imageCollection = await ee.LoadImageCollection(Copernicus.S2_SR_HARMONIZED.Value())
            .FilterByDateRange("2022-01-01", "2022-01-31")
            .FilterByGeometry(new Point { X = -81.282135, Y = -4.453218 })
            .BuildAndFetchAsync();

        #endregion

        #region Assert

        //Assert
        Assert.NotNull(imageCollection);
        Assert.NotEmpty(imageCollection);
        Assert.NotNull(imageCollection[0].Id);
        Assert.NotNull(imageCollection[0].Name);
        Assert.True(imageCollection[0].Properties.CloudCover > 0);
        #endregion
    }
}
