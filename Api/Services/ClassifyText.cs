using Core.Model;
using Core.Services;
using Microsoft.Extensions.Primitives;
using System.Reflection.Metadata.Ecma335;

namespace Api.Services
{
    public class ClassifyText : IClassifyText
    {
        private DataModel _dataModel;
        private TextVector _vector;

        public ClassifyText(DataModel dataModel)
        {
            _dataModel = dataModel;
        }

        public int Classify(string text)
        {
            string cleanText = CleanerData.FullClean(text);
            _vector = VectorizationData.VectorizeSingle(cleanText, _dataModel);

            int result = SVM.PredictStatic(_vector, _dataModel);
            return result;
        }

        public (int label, double confidence) ClassifyWithConfidence(string text)
        {
            int result;
            double confidence;

            string cleanText = CleanerData.FullClean(text);
            _vector = VectorizationData.VectorizeSingle(cleanText, _dataModel);

            (result, confidence) = SVM.PredictWithConfidence(_vector, _dataModel);
            return (result, confidence);
        }

        public List<int> ClassifyBatch(List<string> texts)
        {
            List<int> results = new List<int>();

            foreach(string text in texts)
            {
                results.Add(Classify(text));
            }

            return results;
        }
    }
}
