using Core.Model;
using Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.FunctionalTests
{
    public class SVMTests
    {
        private List<TextVector> CreateSimpleVectors()
        {
            var vectors = new List<TextVector>();

            for (int i = 0; i < 50; i++)
            {
                vectors.Add(new TextVector
                {
                    Label = 1,
                    Indexes = new[] { 0, 1 },
                    Weights = new[] { 1.0, 0.5 }
                });
            }

            for (int i = 0; i < 50; i++)
            {
                vectors.Add(new TextVector
                {
                    Label = 0,
                    Indexes = new[] { 2, 3 },
                    Weights = new[] { 1.0, 0.5 }
                });
            }

            return vectors;
        }

        [Fact]
        public void Train_InitializesWeightsToZero()
        {
            var svm = new SVM(10);
            Assert.All(svm.GetWeights, w => Assert.Equal(0.0, w));
        }

        [Fact]
        public void Train_ChangesWeightsAfterTraining()
        {
            var svm = new SVM(4);
            var vectors = CreateSimpleVectors();
            svm.Train(vectors, epoch: 10);
            Assert.True(svm.GetWeights.Any(w => w != 0.0));
        }

        [Fact]
        public void PredictStatic_ReturnsZeroOrOne()
        {
            var svm = new SVM(4);
            var vectors = CreateSimpleVectors();
            svm.Train(vectors, epoch: 50);

            var model = new DataModel
            {
                SvmWeights = svm.GetWeights,
                SvmBias = svm.GetBias,
                Dictionary = new List<string> { "w0", "w1", "w2", "w3" },
                IdfWeights = new double[] { 1, 1, 1, 1 }
            };

            var testVector = new TextVector
            {
                Indexes = new[] { 0, 1 },
                Weights = new[] { 1.0, 0.5 }
            };

            int result = SVM.PredictStatic(testVector, model);
            Assert.True(result == 0 || result == 1);
        }

        [Fact]
        public void PredictWithConfidence_ConfidenceInRange()
        {
            var svm = new SVM(4);
            var vectors = CreateSimpleVectors();
            svm.Train(vectors, epoch: 50);

            var model = new DataModel
            {
                SvmWeights = svm.GetWeights,
                SvmBias = svm.GetBias,
                Dictionary = new List<string> { "w0", "w1", "w2", "w3" },
                IdfWeights = new double[] { 1, 1, 1, 1 }
            };

            var testVector = new TextVector
            {
                Indexes = new[] { 0, 1 },
                Weights = new[] { 1.0, 0.5 }
            };

            var (label, confidence) = SVM.PredictWithConfidence(testVector, model);
            Assert.True(confidence >= 0.5 && confidence <= 1.0);
        }

        [Fact]
        public void PredictWithConfidence_EmptyVector_ReturnsBiasBasedResult()
        {
            var model = new DataModel
            {
                SvmWeights = new double[] { 1.0, -1.0 },
                SvmBias = 0.5,
                Dictionary = new List<string> { "w0", "w1" },
                IdfWeights = new double[] { 1, 1 }
            };

            var emptyVector = new TextVector
            {
                Indexes = Array.Empty<int>(),
                Weights = Array.Empty<double>()
            };

            var (label, confidence) = SVM.PredictWithConfidence(emptyVector, model);
            Assert.Equal(1, label);
        }
    }
}
