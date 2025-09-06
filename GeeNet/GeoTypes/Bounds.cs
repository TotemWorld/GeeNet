using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeeNet.GeoTypes
{
    public class Bounds : IGeoType
    {
        public required Point Min { get; set; }
        public required Point Max { get; set; }
        public double Buffer { get; set; }
        public Point MiddlePoint { get; set; }

        public ExpandoObject GetArray()
        {
            dynamic expando = new ExpandoObject();
            expando.min = new double[] { Min.X, Min.Y};
            expando.max = new double[] { Max.X, Max.Y};
            return expando;
        }
    }
}
