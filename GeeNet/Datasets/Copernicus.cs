using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeeNet.Datasets
{
    public static class Copernicus
    {
        private static string value = "COPERNICUS";
        public static Dataset S2_SR_HARMONIZED => new Dataset(value, "S2_SR_HARMONIZED");

        public readonly struct Dataset
        {
            private readonly string _program;
            private readonly string _dataset;

            public Dataset(string program, string dataset) { _program = program; _dataset = dataset; }
            public string Value() => $"{_program}/{_dataset}";
        }
    }
}
