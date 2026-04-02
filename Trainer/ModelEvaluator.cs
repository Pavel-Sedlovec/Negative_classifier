using Core.Model;
using Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trainer
{
    public class ModelEvaluator
    {
        public void Evaluate(List<string> testData, List<int> testLabels, DataModel model)
        {
            int tp = 0, fp = 0, fn = 0, tn = 0;
            var errors = new List<(string text, int actual, int predicted)>();

            for (int i = 0; i < testData.Count; i++)
            {
                if (string.IsNullOrEmpty(testData[i])) continue;

                TextVector vector = VectorizationData.VectorizeSingle(testData[i], model);
                int predicted = SVM.PredictStatic(vector, model);
                int actual = testLabels[i];

                if (predicted == 1 && actual == 1) tp++;
                else if (predicted == 1 && actual == 0) fp++;
                else if (predicted == 0 && actual == 1) fn++;
                else tn++;

                if (predicted != actual)
                    errors.Add((testData[i], actual, predicted));
            }

            double precision = tp + fp == 0 ? 0 : (double)tp / (tp + fp);
            double recall = tp + fn == 0 ? 0 : (double)tp / (tp + fn);
            double f1 = precision + recall == 0 ? 0 : 2 * precision * recall / (precision + recall);
            double accuracy = (double)(tp + tn) / (tp + fp + fn + tn);

            Console.WriteLine($"\n=== Результаты ===");
            Console.WriteLine($"Тест: {testData.Count} примеров");
            Console.WriteLine($"Accuracy:  {accuracy:P1}");
            Console.WriteLine($"Precision: {precision:P1}");
            Console.WriteLine($"Recall:    {recall:P1}");
            Console.WriteLine($"F1:        {f1:P1}");
            Console.WriteLine($"TP:{tp} FP:{fp} FN:{fn} TN:{tn}");

            Console.WriteLine($"\n=== Ошибки ({errors.Count}) ===");
            foreach (var (text, actual, predicted) in errors.Take(20))
            {
                string preview = text[..Math.Min(80, text.Length)];
                Console.WriteLine($"[Факт:{actual} , Предсказано:{predicted}] {preview}");
            }
        }
    }
}
