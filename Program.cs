using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestDictionary
{
    class Program
    {
        private static List<string> TestWords { get; set; }
        private static List<string> DictionaryWords { get; set; }
        private static List<string> ResultWords { get; set; } = new List<string>();
        private static readonly List<string> WordNoDictionary = new();
        
        static void Main(string[] args)
        {
            WriteNewTestWords(args[0], args[1], args[2]);
        }

        // Writing words to a new file "NewTestWords.txt" in ...\TestDictionary\bin\Debug\net5.0

        private static void WriteNewTestWords(string pathDictionary, string pathTestWords, string pathNewTestWords)
        {
            NewDictionary(pathDictionary, pathTestWords);
            using (TextWriter tw = new StreamWriter(pathNewTestWords))
            {
                foreach (var word in ResultWords)
                    tw.WriteLine(word);
            }
        }

        private static void NewDictionary(string pathDictionary, string pathTestWords)
        {
            TestWords = ReturnWordsFromFile(pathTestWords);
            DictionaryWords = ReturnWordsFromFile(pathDictionary);
            SearchWordNoInDictionary(TestWords, DictionaryWords);

            foreach (var wordTest in TestWords)
            {
                foreach (var wordDictionary in DictionaryWords)
                {
                    if (wordTest.Equals(wordDictionary, StringComparison.CurrentCultureIgnoreCase))
                    {
                        ResultWords.Add(wordDictionary.ToLower());
                    }

                    else
                    {
                        SearchCompoundWords(wordTest, wordDictionary, DictionaryWords);
                    }
                }
            }

            AddWordsToResult();
        }

        // Finding compound words and splitting them into simple words

        private static void SearchCompoundWords(string wordTest, string wordDictionary, List<string> dictionaryWords)
        {
            var thirdWord = string.Empty;

            if (wordTest.StartsWith(wordDictionary, StringComparison.InvariantCultureIgnoreCase) &&
                wordDictionary.Length > 2)
            {
                var secondPartWord = wordTest.Remove(0, wordDictionary.Length);
                foreach (var dictionaryWord in dictionaryWords)
                {

                    if (secondPartWord.StartsWith(dictionaryWord, StringComparison.InvariantCultureIgnoreCase) &&
                        dictionaryWord.Length > 2)
                    {
                        var thirdPartWord = secondPartWord.Remove(0, dictionaryWord.Length);
                        string secondWord = dictionaryWord;

                        if (thirdPartWord.Length > 2)
                        {
                            foreach (var word in dictionaryWords)
                            {
                                if (thirdPartWord.StartsWith(word, StringComparison.InvariantCultureIgnoreCase) &&
                                    word.Length > 2)
                                {
                                    thirdWord = word;
                                }
                            }
                        }

                        var newWord = wordDictionary + secondWord + thirdWord;
                        if (wordTest.Equals(newWord, StringComparison.CurrentCultureIgnoreCase))
                        {
                            ResultWords.Add($"{wordDictionary.ToLower()} {secondWord.ToLower()} {thirdWord.ToLower()}");
                        }
                    }
                }
            }
        }

        // Adding words that do not have a dictionary

        private static void SearchWordNoInDictionary(List<string> testWords, List<string> dictionary)
        {
            List<string> dictionaryWordsLower = new();
            foreach (var word in dictionary)
            {
                dictionaryWordsLower.Add(word.ToLower());
            }

            var words = testWords.Except(dictionaryWordsLower);
            foreach (var word in words)
            {
                WordNoDictionary.Add(word);
            }
        }

        // Adding words that do not have a dictionary and that cannot be separated

        private static void AddWordsToResult()
        {
            List<string> testWordsToLower = new();
            foreach (var word in ResultWords)
            {
                string newString = Regex.Replace(word, @"\s+", "");
                testWordsToLower.Add(newString.ToLower());
            }

            IEnumerable<string> words = WordNoDictionary.Except(testWordsToLower);
            foreach (var word in words)
            {
                ResultWords.Add(word);
            }

        }

        // Rerurn all words from file

        private static List<string> ReturnWordsFromFile(string path)
        {
            List<string> keywords = File.ReadAllLines(path).ToList();
            return keywords;
        }
    }
}


