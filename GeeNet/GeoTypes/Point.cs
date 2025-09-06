using GeeNet.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeeNet.GeoTypes
{
    public class Point : IGeoType
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Bounds Buffer(double distance)
        {
            var parallelCircumference = WGS84.SEMI_MAJOR_AXIS * Math.Cos(Y * Math.PI / 180) * 2 * Math.PI;
            var meridionalCircumference = WGS84.SEMI_MINOR_AXIS * 2 * Math.PI;

            var latDegreeDistance = meridionalCircumference / 360;
            var lonDegreeDistance = parallelCircumference / 360;

            var latOffset = distance / latDegreeDistance;
            var lonOffset = distance / lonDegreeDistance;
            return new Bounds
            {
                Min = new Point { X = X - lonOffset, Y = Y - latOffset },
                Max = new Point { X = X + lonOffset, Y = Y + latOffset },
                Buffer = distance,
                MiddlePoint = new Point { X = X, Y = Y }
            };
        }
    }
}
