using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Services
{
    public class CleanerData
    {

        private static readonly HashSet<string> _stopWords = new HashSet<string>
        {
            "и", "в", "во", "не", "что", "он", "на", "я", "с", "со", "как", "а", "то", "все", "она",
            "так", "его", "но", "да", "ты", "к", "у", "же", "вы", "за", "бы", "по", "только", "ее",
            "мне", "было", "вот", "от", "меня", "еще", "о", "из", "ему", "теперь", "когда", "даже",
            "ну", "вдруг", "ли", "если", "уже", "или", "ни", "быть", "был", "него", "до", "вас",
            "нибудь", "опять", "уж", "вам", "ведь", "там", "потом", "себя", "ничего", "ей", "может",
            "они", "тут", "где", "есть", "надо", "ней", "для", "мы", "тебя", "их", "чем", "была",
            "сам", "чтоб", "без", "будто", "чего", "раз", "тоже", "себе", "под", "будет", "ж", "тогда",
            "кто", "этот", "того", "потому", "этого", "какой", "совсем", "эти", "здесь", "этом", "один", "почти"
        };


        public static string FullClean(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            string cleaned = text.ToLower();

            cleaned = Regex.Replace(cleaned, @"[^а-яёa-z\s]", " ");

            string[] words = cleaned.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var filteredWords = words
                .Where(w => !_stopWords.Contains(w) && w.Length > 2)
                .ToList();

            return string.Join(" ", filteredWords);
        }
    }
}
