using Microsoft.ML.Data;

namespace Algorithm.Common.ML {
    public class ProductSalesPrediction {
        // Vector to hold Alert, Score, and P-Value values
        [VectorType(9)]
        public double[] Prediction { get; set; }
    }
}
