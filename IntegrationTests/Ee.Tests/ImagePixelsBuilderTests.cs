using GeeNet;
using GeeNet.Datasets;
using GeeNet.Ee;
using GeeNet.GeoTypes;
using Microsoft.Extensions.Options;
using OSGeo.GDAL;
using System.Text.Json;

namespace IntegrationTests;

public class ImagePixelsBuilderTests
{
    [Fact]
    public async Task ImagePixelsBuilder_FullWorkflow_RetrieveImagePixels()
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

        var ee = new Ee(client, iOptions);

        var stream = await ee.LoadImageCollection(Copernicus.S2_SR_HARMONIZED.Value())
            .FilterByDateRange("2022-01-01", "2022-12-31")
            .FilterByGeometry(new Point { X = -81.282135, Y = -4.453218 })
            .ReduceWith("median")
            .SelectBands("B1_median", "B2_median", "B3_median", "B4_median")
            .SetPixelsGrid(new Point { X = -81.282135, Y = -4.453218 }.Buffer(1000), 10)
            .BuildAndFetchAsync();

        Assert.True(await IsValidGeoTiffAsync(stream));
    }

    public async Task<bool> IsValidGeoTiffAsync(Stream responseStream)
    {

        var gdalBinPath = Path.Combine(AppContext.BaseDirectory, "gdal", "bin", "gdal");
        var gdalCSharpPath = Path.Combine(gdalBinPath, "csharp");

        Environment.SetEnvironmentVariable("PATH", $"{gdalCSharpPath};{gdalBinPath}");

        Gdal.AllRegister();

        using var ms = new MemoryStream();
        await responseStream.CopyToAsync(ms);
        var bytes = ms.ToArray();

        string vsimemPath = "/vsimem/temp.tif";
        Gdal.FileFromMemBuffer(vsimemPath, bytes);

        Dataset? ds = null;
        try
        {
            ds = Gdal.Open(vsimemPath, Access.GA_ReadOnly);
            return ds != null;
        }
        catch
        {
            return false;
        }
        finally
        {
            ds?.Dispose();
            Gdal.Unlink(vsimemPath);
        }
    }
}
