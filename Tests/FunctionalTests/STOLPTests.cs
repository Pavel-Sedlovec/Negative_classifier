using Core.Model;
using Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.FunctionalTests
{
    public class STOLPTests
    {
        [Fact]
        public void TextStolp_EmptyList_ReturnsEmpty()
        {
            var stolp = new STOLP();
            var result = stolp.TextStolp(new List<TextVector>());
            Assert.Empty(result);
        }

        [Fact]
        public void GetDistance_IdenticalVectors_ReturnsZero()
        {
            var stolp = new STOLP();
            var v1 = new TextVector { Indexes = new[] { 0, 1 }, Weights = new[] { 1.0, 2.0 } };
            var v2 = new TextVector { Indexes = new[] { 0, 1 }, Weights = new[] { 1.0, 2.0 } };

            double dist = stolp.GetDistance(v1, v2);
            Assert.Equal(0.0, dist, precision: 10);
        }

        [Fact]
        public void GetDistance_OrthogonalVectors_ReturnsCorrectDistance()
        {
            var stolp = new STOLP();
            var v1 = new TextVector { Indexes = new[] { 0 }, Weights = new[] { 3.0 } };
            var v2 = new TextVector { Indexes = new[] { 1 }, Weights = new[] { 4.0 } };

            double dist = stolp.GetDistance(v1, v2);
            Assert.Equal(5.0, dist, precision: 10);
        }

        [Fact]
        public void GetDistance_EmptyVectors_ReturnsZero()
        {
            var stolp = new STOLP();
            var v1 = new TextVector { Indexes = Array.Empty<int>(), Weights = Array.Empty<double>() };
            var v2 = new TextVector { Indexes = Array.Empty<int>(), Weights = Array.Empty<double>() };

            double dist = stolp.GetDistance(v1, v2);
            Assert.Equal(0.0, dist);
        }
    }
}
