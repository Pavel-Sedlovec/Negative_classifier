using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace Core.Model
{
    public class DataModel
    {
        public List<string> Dictionary { get; set; } = new List<string>();
        public double[] IdfWeights { get; set; }
        public double[] SvmWeights { get; set; }
        public double SvmBias { get; set; }

        
        public void Save(string path)
        {
            var json = JsonSerializer.Serialize(this);
            File.WriteAllText(path, json);
        }

        public static DataModel Load(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<DataModel>(json);
        }
    }
}
