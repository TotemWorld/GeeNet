using GeeNet.GeoTypes;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeeNet.Helpers
{
    internal static class UTMHelper
    {
        internal static string GetUtmHemisphere(double latitude)
        {
            return (latitude >= 0) ? "EPSG:326" : "EPSG:327";
        }

        internal static int GetUtmZone(double longitude)
        {
            var zone =  (int)Math.Floor((longitude + 180) / 6) + 1;
            return zone;
        }

        internal static string GetUtmCrsCode(double longitude)
        {
            return GetUtmHemisphere(longitude) + GetUtmZone(longitude);
        }

        internal static Bounds ConvertToUtm(Bounds bounds)
        {
            var csFactory = new CoordinateSystemFactory();
            var ctFactory = new CoordinateTransformationFactory();

            var isNorthern = bounds.MiddlePoint.Y >= 0;
            var utmZone = GetUtmZone(bounds.MiddlePoint.X);

            var wgs84 = GeographicCoordinateSystem.WGS84;

            var utm = ProjectedCoordinateSystem.WGS84_UTM(utmZone, isNorthern);
            var transform = ctFactory.CreateFromCoordinateSystems(wgs84, utm);

            dynamic boundsArray = bounds.GetArray();

            var minXY = (double[])transform.MathTransform.Transform(boundsArray.min);
            var maxXY = (double[])transform.MathTransform.Transform(boundsArray.max);

            return new Bounds
            {
                Min = new Point { X = minXY[0], Y = minXY[1] },
                Max = new Point { X = maxXY[0], Y = maxXY[1] },
                Buffer = bounds.Buffer,
                MiddlePoint = new Point { X = (minXY[0] + maxXY[0]) / 2, Y = (minXY[1] + maxXY[1]) / 2 }
            };

        }
    }
}
