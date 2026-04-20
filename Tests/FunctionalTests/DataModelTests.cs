using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.FunctionalTests
{
    public class DataModelTests
    {
        [Fact]
        public void SaveAndLoad_PreservesData()
        {
            var model = new DataModel
            {
                Dictionary = new List<string> { "слово1", "слово2", "слово3" },
                IdfWeights = new double[] { 1.5, 2.0, 0.8 },
                SvmWeights = new double[] { 0.3, -0.5, 0.1 },
                SvmBias = 0.05
            };

            string path = Path.Combine(Path.GetTempPath(), "test_model.json");

            try
            {
                model.Save(path);
                var loaded = DataModel.Load(path);

                Assert.Equal(model.Dictionary, loaded.Dictionary);
                Assert.Equal(model.IdfWeights, loaded.IdfWeights);
                Assert.Equal(model.SvmWeights, loaded.SvmWeights);
                Assert.Equal(model.SvmBias, loaded.SvmBias);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [Fact]
        public void Load_InvalidPath_ThrowsException()
        {
            Assert.Throws<FileNotFoundException>(() =>
                DataModel.Load("несуществующий_путь.json"));
        }
    }
}
