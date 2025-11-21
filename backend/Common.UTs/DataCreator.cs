using Algorithm.Common.ML;
using Microsoft.ML;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Data;

namespace Common.UTs {
    public class DataCreator {
        private const string SolutionAPath = @"R:\SolutionA";
        private const string SolutionBPath = @"R:\SolutionB";
        private const string SolutionCPath = @"R:\SolutionC";

        [Fact]
        public void Create_folders() {
            //given
            string pathA = SolutionAPath;
            string pathB = SolutionBPath;
            string pathC = SolutionCPath;

            //when
            Directory.CreateDirectory(pathA);
            Directory.CreateDirectory(pathB);
            Directory.CreateDirectory(pathC);

            //then
            bool existsA = Directory.Exists(pathA);
            bool existsB = Directory.Exists(pathA);
            bool existsC = Directory.Exists(pathA);

            Assert.True(existsA);
            Assert.True(existsB);
            Assert.True(existsC);
        }

        private static string BaseDatasetsRelativePath = @"../../../Data";
        private static string DatasetRelativePath = $"{BaseDatasetsRelativePath}/Product-sales.csv";

        private static string DatasetPath = GetAbsolutePath(DatasetRelativePath);

        private static string ModelRelativePath1 = $"R:/SolutionA/ProductSalesSpikeModel.zip";
        private static string ModelRelativePath2 = $"R:/SolutionB/ProductSalesChangePointModel.zip";

        private static string SpikeModelPath = ModelRelativePath1;
        private static string ChangePointModelPath = ModelRelativePath2;

        private static MLContext mlContext;

        [Fact]
        public void Create_models() {
            //given
            mlContext = new MLContext();

            // Assign the Number of records in dataset file to constant variable.
            const int size = 36;

            // Load the data into IDataView.
            // This dataset is used for detecting spikes or changes not for training.
            IDataView dataView = mlContext.Data.LoadFromTextFile<ProductSalesData>(path: DatasetPath, hasHeader: true, separatorChar: ',');

            // Detect temporary changes (spikes) in the pattern.
            ITransformer trainedSpikeModel = DetectSpike(size, dataView);

            // Detect persistent change in the pattern.
            //when
            ITransformer trainedChangePointModel = DetectChangepoint(size, dataView);

            SaveModel(mlContext, trainedSpikeModel, SpikeModelPath, dataView);
            SaveModel(mlContext, trainedChangePointModel, ChangePointModelPath, dataView);
        }

        private static ITransformer DetectSpike(int size, IDataView dataView) {
            Console.WriteLine("===============Detect temporary changes in pattern===============");

            // STEP 1: Create Estimator.
            var estimator = mlContext.Transforms.DetectIidSpike(outputColumnName: nameof(ProductSalesPrediction.Prediction), inputColumnName: nameof(ProductSalesData.numSales), confidence: 95, pvalueHistoryLength: size / 4);

            // STEP 2:The Transformed Model.
            // In IID Spike detection, we don't need to do training, we just need to do transformation. 
            // As you are not training the model, there is no need to load IDataView with real data, you just need schema of data.
            // So create empty data view and pass to Fit() method. 
            ITransformer tansformedModel = estimator.Fit(CreateEmptyDataView());

            // STEP 3: Use/test model.
            // Apply data transformation to create predictions.
            IDataView transformedData = tansformedModel.Transform(dataView);
            var predictions = mlContext.Data.CreateEnumerable<ProductSalesPrediction>(transformedData, reuseRowObject: false);

            Console.WriteLine("Alert\tScore\tP-Value");
            foreach (var p in predictions) {
                if (p.Prediction[0] == 1) {
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.WriteLine("{0}\t{1:0.00}\t{2:0.00}", p.Prediction[0], p.Prediction[1], p.Prediction[2]);
                Console.ResetColor();
            }
            Console.WriteLine("");
            return tansformedModel;
        }

        private static ITransformer DetectChangepoint(int size, IDataView dataView) {
            Console.WriteLine("===============Detect Persistent changes in pattern===============");

            // STEP 1: Setup transformations using DetectIidChangePoint.
            var estimator = mlContext.Transforms.DetectIidChangePoint(outputColumnName: nameof(ProductSalesPrediction.Prediction), inputColumnName: nameof(ProductSalesData.numSales), confidence: 95, changeHistoryLength: size / 4);

            // STEP 2:The Transformed Model.
            // In IID Change point detection, we don't need need to do training, we just need to do transformation. 
            // As you are not training the model, there is no need to load IDataView with real data, you just need schema of data.
            // So create empty data view and pass to Fit() method.  
            ITransformer tansformedModel = estimator.Fit(CreateEmptyDataView());

            // STEP 3: Use/test model.
            // Apply data transformation to create predictions.
            IDataView transformedData = tansformedModel.Transform(dataView);
            var predictions = mlContext.Data.CreateEnumerable<ProductSalesPrediction>(transformedData, reuseRowObject: false);

            Console.WriteLine($"{nameof(ProductSalesPrediction.Prediction)} column obtained post-transformation.");
            Console.WriteLine("Alert\tScore\tP-Value\tMartingale value");

            foreach (var p in predictions) {
                if (p.Prediction[0] == 1) {
                    Console.WriteLine("{0}\t{1:0.00}\t{2:0.00}\t{3:0.00}  <-- alert is on, predicted changepoint", p.Prediction[0], p.Prediction[1], p.Prediction[2], p.Prediction[3]);
                } else {
                    Console.WriteLine("{0}\t{1:0.00}\t{2:0.00}\t{3:0.00}", p.Prediction[0], p.Prediction[1], p.Prediction[2], p.Prediction[3]);
                }
            }
            Console.WriteLine("");
            return tansformedModel;
        }

        private static void SaveModel(MLContext mlcontext, ITransformer trainedModel, string modelPath, IDataView dataView) {
            Console.WriteLine("=============== Saving model ===============");
            mlcontext.Model.Save(trainedModel, dataView.Schema, modelPath);

            Console.WriteLine($"The model is saved to {modelPath}");
        }

        public static string GetAbsolutePath(string relativePath) {
            var _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }

        private static IDataView CreateEmptyDataView() {
            //Create empty DataView. We just need the schema to call fit()
            IEnumerable<ProductSalesData> enumerableData = new List<ProductSalesData>();
            var dv = mlContext.Data.LoadFromEnumerable(enumerableData);
            return dv;
        }
    }
}
