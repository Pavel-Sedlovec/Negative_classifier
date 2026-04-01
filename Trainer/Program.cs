using Core.Model;
using Core.Services;
using System.IO;
using System.Reflection.Emit;

namespace Trainer
{
    internal class Program
    {
        private static string _path = @"D:\labeled.csv";
        private static string _savePath = "D:\\Мальков 2 курс\\Negative_classifier\\Api\\Data\\dataTest.json";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            TrainModel trainModel = new TrainModel(_savePath);
            trainModel.TrainWithEvaluation(_path);

            DataModel model = DataModel.Load(_savePath);

            Console.WriteLine($"Слов в модели: {model.Dictionary.Count}");
           
            string test = CleanerData.FullClean("пиздец позор зоопарк ебаный чиж говно человек ещё хуже");
            Console.WriteLine($"После очистки: '{test}'");
            var words = test.Split(' ');
            foreach (var w in words)
                Console.WriteLine($"  '{w}' → в словаре: {model.Dictionary.Contains(w)}");

            Console.WriteLine($"Bias: {model.SvmBias}");
            Console.WriteLine($"Weights max: {model.SvmWeights.Max()}");
            Console.WriteLine($"Weights min: {model.SvmWeights.Min()}");
            Console.WriteLine($"Weights avg: {model.SvmWeights.Average()}");

            string clean = CleanerData.FullClean("пиздец позор зоопарк ебаный чиж говно человек ещё хуже");
            double[] vec = VectorizationData.VectorizeSingle(clean, model);
            double raw = vec.Zip(model.SvmWeights, (v, w) => v * w).Sum() + model.SvmBias;
            Console.WriteLine($"Raw score: {raw}");

        }        
    }    
}
