using Core.Model;
using Core.Services;
using System.IO;
using System.Reflection.Emit;

namespace Trainer
{
    internal class Program
    {
        private static string _path = @"D:\labeled.csv";
        //private static string _path = @"C:\Users\Pasha\Downloads\parsed_test3_1.csv";

        private static string _savePath = "D:\\Мальков 2 курс\\Negative_classifier\\Api\\Data\\dataTest2.json";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            TrainModel trainModel = new TrainModel(_savePath);
            //trainModel.TrainWithEvaluation(_path);
            trainModel.TrainSVM(_path);

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


            string clean = CleanerData.FullClean("привет");
            Console.WriteLine($"После очистки: '{clean}'");

            var vector = VectorizationData.VectorizeSingle(clean, model);
            Console.WriteLine($"Ненулевых весов: {vector.Indexes.Length}");

            double raw = 0;
            for (int i = 0; i < vector.Indexes.Length; i++)
                raw += vector.Weights[i] * model.SvmWeights[vector.Indexes[i]];
            raw += model.SvmBias;
            Console.WriteLine($"Raw score: {raw}");
            Console.WriteLine($"Bias: {model.SvmBias}");

        }        
    }    
}
