using Core.Model;
using Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.FunctionalTests
{
    public class VectorizationDataTests
    {
        private List<string> GetTestData() => new List<string>
        {
            "хороший товар быстрая доставка",
            "плохой товар сломался сразу",
            "отличное качество рекомендую всем",
            "ужасный продукт деньги выброшены"
        };

        private List<int> GetTestLabels() => new List<int> { 0, 1, 0, 1 };

        [Fact]
        public void TF_IDF_ReturnsCorrectCount()
        {
            var vd = new VectorizationData(GetTestData(), GetTestLabels());
            var vectors = vd.TF_IDF();
            Assert.Equal(GetTestData().Count, vectors.Count);
        }

        [Fact]
        public void TF_IDF_DictionaryNotEmpty()
        {
            var vd = new VectorizationData(GetTestData(), GetTestLabels());
            vd.TF_IDF();
            Assert.NotEmpty(vd.Dictionary);
        }

        [Fact]
        public void TF_IDF_IdfWeightsLengthMatchesDictionary()
        {
            var vd = new VectorizationData(GetTestData(), GetTestLabels());
            vd.TF_IDF();
            Assert.Equal(vd.Dictionary.Count, vd.Idf.Length);
        }

        [Fact]
        public void TF_IDF_VectorsHaveCorrectLabels()
        {
            var labels = GetTestLabels();
            var vd = new VectorizationData(GetTestData(), labels);
            var vectors = vd.TF_IDF();

            for (int i = 0; i < vectors.Count; i++)
                Assert.Equal(labels[i], vectors[i].Label);
        }

        [Fact]
        public void TF_IDF_SparseIndexesAreSorted()
        {
            var vd = new VectorizationData(GetTestData(), GetTestLabels());
            var vectors = vd.TF_IDF();

            foreach (var vector in vectors)
            {
                for (int i = 1; i < vector.Indexes.Length; i++)
                    Assert.True(vector.Indexes[i] > vector.Indexes[i - 1]);
            }
        }

        [Fact]
        public void VectorizeSingle_KnownWord_NonZeroWeight()
        {
            var vd = new VectorizationData(GetTestData(), GetTestLabels());
            vd.TF_IDF();

            var model = new DataModel
            {
                Dictionary = vd.Dictionary,
                IdfWeights = vd.Idf,
                SvmWeights = new double[vd.Dictionary.Count],
                SvmBias = 0
            };

            string clean = CleanerData.FullClean("хороший товар");
            var vector = VectorizationData.VectorizeSingle(clean, model);

            Assert.True(vector.Indexes.Length > 0);
        }

        [Fact]
        public void VectorizeSingle_UnknownWords_EmptyVector()
        {
            var vd = new VectorizationData(GetTestData(), GetTestLabels());
            vd.TF_IDF();

            var model = new DataModel
            {
                Dictionary = vd.Dictionary,
                IdfWeights = vd.Idf,
                SvmWeights = new double[vd.Dictionary.Count],
                SvmBias = 0
            };

            var vector = VectorizationData.VectorizeSingle("xyzxyzxyz qqqqqq", model);
            Assert.Empty(vector.Indexes);
        }
    }
}
