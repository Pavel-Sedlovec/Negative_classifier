using Core.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class VectorizationData
    {
        private double[] idf;
        private List<string> dictionary = new List<string>();
        private List<string> data = new List<string>();
        private List<int> lable = new List<int>();

        public VectorizationData(List<string> data, List<int> lable)
        {
            this.data = data;
            this.lable = lable;
        }

        public List<string> Dictionary
        {
            get {  return this.dictionary; }
        }

        public double[] Idf
        {
            get { return this.idf; }
        }

        //private void CreatDictionary()
        //{
        //    var uniqueWords = new HashSet<string>();
        //    foreach (var doc in data)
        //    {
        //        foreach (var word in doc.Split(' ')) uniqueWords.Add(word);
        //    }
        //    dictionary = uniqueWords.ToList();
        //}

        private void CreatDictionary()
        {
            var wordCount = new Dictionary<string, int>();
            foreach (var doc in data)
            {
                var words = doc.Split(' ');
                foreach (var word in words)
                    wordCount[word] = wordCount.GetValueOrDefault(word) + 1;

                //for(int i = 0; i < words.Length - 1; i++)
                //{
                //    string bigram = words[i] + "_" + words[i+1];
                //    wordCount[bigram] = wordCount.GetValueOrDefault(bigram) + 1;
                //}
            }

            dictionary = wordCount
                .OrderByDescending(kv => kv.Value)
                .Take(15000)
                .Select(kv => kv.Key)
                .ToList();
        }


        //private void IDF()
        //{
        //    double[] docCounts = new double[dictionary.Count];
        //    idf = new double[dictionary.Count];

        //    for (int i = 0; i < dictionary.Count; i++)
        //    {
        //        string token = dictionary[i];
        //        bool isBigram = token.Contains('_');

        //        if (!isBigram)
        //        {
        //            docCounts[i] = data.Count(d => d.Split(' ').Contains(token));
        //        }
        //        else
        //        {
        //            foreach (var doc in data)
        //            {
        //                var words = doc.Split(' ');
        //                for (int k = 0; k < words.Length - 1; k++)
        //                {
        //                    if (words[k] + "_" + words[k + 1] == token)
        //                    {
        //                        docCounts[i]++;
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    for (int i = 0; i < dictionary.Count; i++)
        //    {
        //        idf[i] = Math.Log((double)data.Count / (1.0 + docCounts[i]));
        //    }
        //}

        private void IDF()
        {
            double[] docCounts = new double[dictionary.Count];
            for (int i = 0; i < dictionary.Count; i++)
            {
                string word = dictionary[i];
                docCounts[i] = data.Count(d => d.Split(' ').Contains(word));
            }
            idf = new double[dictionary.Count];
            for (int i = 0; i < dictionary.Count; i++)
            {
                idf[i] = Math.Log((double)data.Count / (1.0 + docCounts[i]));
            }
        }

        public List<TextVector> TF_IDF()
        {
            CreatDictionary();
            IDF();
            List<TextVector> textVectors = new List<TextVector>();   
            
            for (int i = 0; i < data.Count; i++)
            {                
                string doc = data[i];
                string[] wordsInDoc = doc.Split(' ');
                //List<string> wordsBigram = new List<string>();

                TextVector vector = new TextVector();

                List<int> indexes = new List<int>();
                List<double> weights = new List<double>();

                vector.Text = data[i];
                vector.Features = new double[dictionary.Count];
                vector.Label = lable[i];

                //for (int k = 0; k < wordsInDoc.Length - 1; k++)
                //{
                //    wordsBigram.Add(wordsInDoc[k] + "_" + wordsInDoc[k + 1]);
                //}

                for (int j = 0; j < dictionary.Count; j++)
                {
                    string wordFromDictionary = dictionary[j];

                    //int countOneGram = wordsInDoc.Count(w => w == wordFromDictionary);
                    //int countBigram = wordsBigram.Count(w => w == wordFromDictionary);
                    //int count = countOneGram + countBigram;

                    int count = wordsInDoc.Count(w => w == wordFromDictionary);

                    if(count != 0)
                    {
                        double tf = (double)count / wordsInDoc.Length;
                        indexes.Add(j);
                        weights.Add(tf * idf[j]);
                    }                                        
                }
                vector.Indexes = indexes.ToArray();
                vector.Weights = weights.ToArray();
                textVectors.Add(vector);
            }
            return textVectors;
        }

        public static TextVector VectorizeSingle(string cleanText, DataModel model)
        {
            string[] wordsInDoc = cleanText.Split(' ');
            //List<string> wordsBigram = new List<string>();

            TextVector vector = new TextVector();

            //for(int i = 0; i < wordsInDoc.Length - 1; i++)
            //{
            //    wordsBigram.Add(wordsInDoc[i] + "_" + wordsInDoc[i + 1]);
            //}

            List<int> indexes = new List<int>();
            List<double> weights = new List<double>();

            for (int j = 0; j < model.Dictionary.Count; j++)
            {
                string wordFromDictionary = model.Dictionary[j];

                //int countOneGram = wordsInDoc.Count(w => w == wordFromDictionary);
                //int countBigram = wordsBigram.Count(w => w == wordFromDictionary);
                //int count = countOneGram+ countBigram;

                int count = wordsInDoc.Count(w => w == wordFromDictionary);

                if(count != 0)
                {
                    double tf = (double)count / wordsInDoc.Length;

                    indexes.Add(j);
                    weights.Add(tf * model.IdfWeights[j]);
                }               
            }
            vector.Indexes = indexes.ToArray();
            vector.Weights = weights.ToArray();
            return vector;
        }
    }
}
