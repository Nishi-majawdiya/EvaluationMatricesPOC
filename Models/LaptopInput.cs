using Microsoft.ML.Data;

namespace EvaluationMatricesPOC.Models
{
    public class LaptopInput
    {
        [LoadColumn(0)]
        public string Brand { get; set; }

        [LoadColumn(1)]
        public float RAM { get; set; }

        [LoadColumn(2)]
        public float Storage { get; set; }

        [LoadColumn(3)]
        public string Processor { get; set; }

        [LoadColumn(4)]
        public float ScreenSize { get; set; }

        [LoadColumn(5)]
        public float Price { get; set; }
    }
}