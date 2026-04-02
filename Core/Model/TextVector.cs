using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{

    public class TextVector
    {
        private string _text;
        private double[] _features;

        public int Label { get; set; }
        public int[] Indexes { get; set; }
        public double[] Weights { get; set; }


        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public double[] Features
        {
            get { return _features; }
            set { _features = value; }
        }

    }
}
