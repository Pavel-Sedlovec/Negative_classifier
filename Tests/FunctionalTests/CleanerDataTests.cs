using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model;
using Core.Services;
using Xunit;

namespace Tests.FunctionalTests
{
    public class CleanerDataTests
    {
        [Fact]
        public void FullClean_NullOrEmpty_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, CleanerData.FullClean(null));
            Assert.Equal(string.Empty, CleanerData.FullClean(""));
            Assert.Equal(string.Empty, CleanerData.FullClean("   "));
        }

        [Fact]
        public void FullClean_RemovesSpecialCharacters()
        {
            string result = CleanerData.FullClean("привет!!! как дела???");
            Assert.DoesNotContain("!", result);
            Assert.DoesNotContain("?", result);
        }

        [Fact]
        public void FullClean_LowercasesText()
        {
            string result = CleanerData.FullClean("ПРИВЕТ МИР");
            Assert.Equal(result, result.ToLower());
        }

        [Fact]
        public void FullClean_RemovesShortWords()
        {
            string result = CleanerData.FullClean("я ты он мир");
            var words = result.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Assert.All(words, w => Assert.True(w.Length > 2));
        }

        [Fact]
        public void FullClean_NormalText_ReturnsCleanedWords()
        {
            string result = CleanerData.FullClean("это хорошая статья");
            Assert.Contains("хорошая", result);
            Assert.Contains("статья", result);
        }
    }
}
