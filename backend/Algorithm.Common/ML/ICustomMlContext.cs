namespace Algorithm.Common.ML {
    public interface ICustomMlContext {
        bool DetectAnomaliesBychangePoint(IList<ProductSalesData> dataFromDatabase, string modelPath);
        bool DetectAnomaliesBySpike(IList<ProductSalesData> dataFromDatabase, string modelPath);
    }
}
