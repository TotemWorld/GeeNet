```
var services = new ServiceCollection();

services.AddGeeNet(options =>
{
    options.ApiVersion = "v1";
});

var provider = services.BuildServiceProvider();

var ee = provider.GetRequiredService<Ee>();

var imageCollection = await ee.LoadImageCollection(Copernicus.S2_SR_HARMONIZED.Value())
    .FilterByDateRange("2022-01-01", "2022-01-31")
    .FilterByGeometry(new Point { X = -81.282135, Y = -4.453218 })
    .BuildAndFetchAsync();

var geoTiffStream = await ee.LoadImageCollection(Copernicus.S2_SR_HARMONIZED.Value())
    .FilterByDateRange("2022-01-01", "2022-12-31")
    .FilterByGeometry(new Point { X = -81.282135, Y = -4.453218 })
    .ReduceWith("median")
    .SelectBands("B1_median", "B2_median", "B3_median", "B4_median")
    .SetPixelsGrid(new Point { X = -81.282135, Y = -4.453218 }.Buffer(1000), 10)
    .BuildAndFetchAsync();
```
