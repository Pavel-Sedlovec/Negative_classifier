using Core.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{   
    public class SVM
    {
        private double[] _weights;
        private double _b;
        private int _size;

        public SVM(int size)
        {
            _size = size;
            _weights = new double[_size];
        }

        public double[] GetWeights
        {
            get { return _weights; }
        }

        public double GetBias
        {
            get { return _b; }
        }

        public void Train(List<TextVector> vector, double learningRate = 0.001, double lambda = 0.01, int epoch = 100)
        {
            double totalPos = 0;
            double totalNeg = 0;
            for(int i = 0; i < vector.Count; i++)
            {
                if (vector[i].Label == 1) totalPos++;
                else totalNeg++;
            }
            double weightPos = (totalPos + totalNeg) / (2.0 * totalPos); // ~1.99
            double weightNeg = (totalPos + totalNeg) / (2.0 * totalNeg); // ~0.73

            for (int i = 0; i < epoch; i++)
            {
                for (int j = 0; j < vector.Count; j++)
                {
                    int svmLabel = vector[j].Label == 1 ? 1 : -1;
                    double classWeight = vector[j].Label == 1 ? weightPos : weightNeg;
                    double prediction = PredictRaw(vector[j]);

                    if (prediction * svmLabel >= 1)
                    {
                        for (int k = 0; k < vector[j].Indexes.Length; k++)
                            _weights[vector[j].Indexes[k]] -= learningRate * (2 * lambda * _weights[vector[j].Indexes[k]]);
                    }
                    else
                    {
                        for (int k = 0; k < vector[j].Indexes.Length; k++)
                            _weights[vector[j].Indexes[k]] += learningRate * classWeight * 
                                (vector[j].Weights[k] * svmLabel - 2 * lambda * _weights[vector[j].Indexes[k]]);
                        _b += learningRate * classWeight * svmLabel;

                    }
                }
            }
        }

        public int Predict(double[] input)
        {
            double prediction = 0;
            for (int i = 0; i < _size; i++)            
                prediction += input[i] * _weights[i];
            prediction += _b;
            
            if (prediction >= 0) return 0;
            else return 1;
        }

        private double PredictRaw(TextVector vector)
        {
            double prediction = 0;
            for (int i = 0; i < vector.Indexes.Length; i++)           
                prediction += vector.Weights[i] * _weights[vector.Indexes[i]];
            
            return prediction + _b;
        }

        public static int PredictStatic(TextVector vector, DataModel model)
        {
            double prediction = GetPrediction(vector, model);

            if (prediction >= 0) return 1;
            else return 0;
        }

        public static (int predict, double confidence) PredictWithConfidence(TextVector vector, DataModel model)
        {
            int label;
            double confidence;
            double prediction = GetPrediction(vector, model);

            if (prediction >= 0) label = 1;
            else label = 0;

            confidence = 1.0 / (1.0 + Math.Exp(-Math.Abs(prediction)));

            return (label, confidence);
        }

        private static double GetPrediction(TextVector vector, DataModel model)
        {
            double prediction = 0;
            for (int i = 0; i < vector.Indexes.Length; i++)
                prediction += vector.Weights[i] * model.SvmWeights[vector.Indexes[i]];
            prediction += model.SvmBias;
            return prediction;
        }
    }
}
