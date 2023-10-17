using System.Linq;
using System.Text;
using Utilities.Con.Models;

namespace Utilities.Con
{
    public class Program
    {
        public static bool IsHiragana(char c)
        {
            return c >= 0x3040 && c <= 0x309F;
        }

        public static bool IsKatakana(char c)
        {
            return c >= 0x30A0 && c <= 0x30FF;
        }
        public static bool IsKanji(char c)
        {
            return c >= 0x4E00 && c <= 0x9FBF || c == '々';
        }

        public static string ExtractKanjiOnly(string s)
        {
            string result = string.Empty;

            foreach (var c in s)
            {
                if (IsKanji(c))
                {
                    result += c;
                }
            }

            return result;
        }

        public static int MaxKanjiLevel(string kanjiWord, Kanji[] kanjiList)
        {
            if (!kanjiWord.Any())
            {
                return 0;
            }
            else
            {
                return kanjiWord.Select(c => kanjiList.FirstOrDefault(k => k.KanjiChar[0] == c)?.Grade ?? 13).Max();
            }
        }

        public static void Main(string[] args)
        {
            var kanji = KanjiTest.KanjiList
                                 .Kanji;

            var vocabList = VocabTest.VocabList
                                 .Vocab
                                 .Select(v => new { KanjiOnly = ExtractKanjiOnly(v.Kanji), Original = v })
                                 .Select(v => new
                                 {
                                     v.Original.Kanji,
                                     v.Original.Kana,
                                     v.Original.English,
                                     v.Original.Level,
                                     NumKanji = ExtractKanjiOnly(v.KanjiOnly).Count(),
                                     MaxKanjiLevel = MaxKanjiLevel(v.KanjiOnly, kanji)
                                 })
                                 .Where(v => v.Kana.EndsWith("いる") || v.Kana.EndsWith("える")).ToList();

            StringBuilder csv = new StringBuilder();

            foreach (var vocab in vocabList)
            {
                csv.AppendLine($"\"{vocab.Kanji.Replace("\"","\"\"")}\",\"{vocab.Kana.Replace("\"", "\"\"")}\",\"{vocab.English.Replace("\"", "\"\"")}\",{vocab.Level},{vocab.NumKanji},{vocab.MaxKanjiLevel}");
            }

            string result = csv.ToString();
        }
    }
}
