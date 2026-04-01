using Core.Model;
using Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trainer
{
    public class TrainModel
    {
        private string _savePath;

        public TrainModel(string savePath)
        {
            _savePath = savePath;
        }

        public void TrainSVM(string dataPath)
        {
            try
            {
                var (rawData, rawLabels) = GetAllTextAndLable(dataPath);

                var (trainData, trainLabels, dataModel) = VectorizeData(rawData, rawLabels);

                TrainAndSaveSVM(trainData, trainLabels, dataModel);               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        public (List<string>, List<int>) GetAllTextAndLable(string path)
        {
            var loader = new DatasetLoader();
            var (data, labels) = loader.LoadCsv(path);

            var cleanedData = new List<string>();
            var cleanedLabels = new List<int>();

            for (int i = 0; i < data.Count; i++)
            {
                var cleaned = CleanerData.FullClean(data[i]);
                if (!string.IsNullOrWhiteSpace(cleaned))
                {
                    cleanedData.Add(cleaned);
                    cleanedLabels.Add(labels[i]);
                }
            }

            return (cleanedData, cleanedLabels);
        }

        public (List<double[]>, int[], DataModel) VectorizeData(List<string> data, List<int> lable)
        {
            VectorizationData vd = new VectorizationData(data, lable);
            DataModel dm = new DataModel();
            var textVectors = vd.TF_IDF();

            dm.Dictionary = vd.Dictionary;
            dm.IdfWeights = vd.Idf;

            //STOLP stolp = new STOLP();
            //textVectors = stolp.TextStolp(textVectors);

            var trainData = textVectors.Select(v => v.Features).ToList();
            var trainLabels = textVectors.Select(v => v.Label).ToArray();

            return (trainData, trainLabels, dm);
        }

        public void TrainAndSaveSVM(List<double[]> trainData, int[] trainLabels, DataModel dm)
        {
            SVM svm = new SVM(dm.Dictionary.Count); 
            svm.Train(trainData, trainLabels);

            dm.SvmWeights = svm.GetWeights;
            dm.SvmBias = svm.GetBias;

            dm.Save(_savePath);
        }


        public void TrainWithEvaluation(string dataPath)
        {
            try
            {
                var (rawData, rawLabels) = GetAllTextAndLable(dataPath);

                var rng = new Random(42);
                var indices = Enumerable.Range(0, rawData.Count).OrderBy(_ => rng.Next()).ToList();
                var shuffledData = indices.Select(i => rawData[i]).ToList();
                var shuffledLabels = indices.Select(i => rawLabels[i]).ToList();

                int trainSize = (int)(shuffledData.Count * 0.8);
                var trainRawData = shuffledData.Take(trainSize).ToList();
                var trainRawLabels = shuffledLabels.Take(trainSize).ToList();
                var testRawData = shuffledData.Skip(trainSize).ToList();
                var testRawLabels = shuffledLabels.Skip(trainSize).ToList();

                var (trainData, trainLabels, dataModel) = VectorizeData(trainRawData, trainRawLabels);
                TrainAndSaveSVM(trainData, trainLabels, dataModel);

                var model = DataModel.Load(_savePath);
                var evaluator = new ModelEvaluator();
                evaluator.Evaluate(testRawData, testRawLabels, model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
