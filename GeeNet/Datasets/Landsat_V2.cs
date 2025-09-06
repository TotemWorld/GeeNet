using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeeNet.Datasets
{
    public static class Landsat
    {
        private static string value = "LANDSAT";
        public static Sensor LC08 => new Sensor(value, "LC08");

        public readonly struct Sensor
        {
            private readonly string _program;
            private readonly string _sensor;
            public Sensor(string provider, string sensor) { _program = value; _sensor = sensor; }

            public Collection C02 => new Collection(_program, _sensor, "C02");
        }

        public readonly struct Collection
        {
            private readonly string _program;
            private readonly string _sensor;
            private readonly string _collection;
            public Collection(string provider, string sensor, string collection)
            { _program = provider; _sensor = sensor; _collection = collection; }

            public Tier T2_L2 => new Tier(_program, _sensor, _collection, "T2_L2");
            public Tier T1_L2 => new Tier(_program, _sensor, _collection, "T1_L2");
        }

        public readonly struct Tier
        {
            private readonly string _program;
            private readonly string _sensor;
            private readonly string _collection;
            private readonly string _tier;
            public Tier(string program, string sensor, string collection, string tier)
            { _program = program; _sensor = sensor; _collection = collection; _tier = tier; }
            public string Value() => $"{_program}/{_sensor}/{_collection}/{_tier}";
        }


    }
}
