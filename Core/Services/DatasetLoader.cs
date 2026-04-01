using Core.Model;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Core.Services
{
    public class DatasetLoader
    {
        public (List<string> data, List<int> label) LoadCsv(string path)
        {
            List<string> data = new List<string>();
            List<int> label = new List<int>();

            using (var parser = new TextFieldParser(path, Encoding.UTF8))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;
                parser.TrimWhiteSpace = false;

                int count = 0;
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    if (count++ == 0) continue;
                    if (fields.Length < 2) continue;

                    string raw = fields[1].Trim();
                    if (!double.TryParse(raw, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out double val))
                        continue;

                    data.Add(fields[0]);
                    label.Add((int)val);
                }
            }
            return (data, label);
        }
    }
}
