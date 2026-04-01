using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public void Train(List<double[]> inputs, int[] labels, double learningRate = 0.001, double lambda = 0.01, int epoch = 100)
        {
            double totalPos = labels.Count(l => l == 1);
            double totalNeg = labels.Count(l => l == 0);
            double weightPos = (totalPos + totalNeg) / (2.0 * totalPos); // ~1.99
            double weightNeg = (totalPos + totalNeg) / (2.0 * totalNeg); // ~0.73

            for (int i = 0; i < epoch; i++)
            {
                for (int j = 0; j < inputs.Count; j++)
                {
                    int svmLabel = labels[j] == 1 ? 1 : -1;
                    double classWeight = labels[j] == 1 ? weightPos : weightNeg;
                    double prediction = PredictRaw(inputs[j]);

                    if (prediction * svmLabel >= 1)
                    {
                        for (int k = 0; k < _size; k++)
                            _weights[k] -= learningRate * (2 * lambda * _weights[k]);
                    }
                    else
                    {
                        for (int k = 0; k < _size; k++)
                            _weights[k] += learningRate * classWeight * (inputs[j][k] * svmLabel - 2 * lambda * _weights[k]);
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

        private double PredictRaw(double[] signs)
        {
            double prediction = 0;
            for (int i = 0; i < _size; i++)           
                prediction += signs[i] * _weights[i];
            
            return prediction + _b;
        }

        public static int PredictStatic(double[] input, DataModel model)
        {
            double prediction = 0;
            for (int i = 0; i < model.SvmWeights.Length; i++)
                prediction += input[i] * model.SvmWeights[i];
            prediction += model.SvmBias;

            if (prediction >= 0) return 1;
            else return 0;
        }
    }
}
