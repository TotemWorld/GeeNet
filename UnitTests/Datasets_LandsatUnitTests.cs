using GeeNet.Datasets;
using System.Text.Json;

namespace UnitTests
{
    public class Datasets_LandsatUnitTests
    {
        [Fact]
        public void Landsat_Tier_Value_ReturnsExpectedFormat()
        {
            var value = Landsat.LC08.C02.T2_L2.Value();
            Assert.Equal("LANDSAT/LC08/C02/T2_L2", value);
        }

        [Fact]
        public void Copernicus_Tier_Value_ReturnsExpectedFormat()
        {
            var value = Copernicus.S2_SR_HARMONIZED.Value();
            Assert.Equal("COPERNICUS/S2_SR_HARMONIZED", value);
        }
    }
}
