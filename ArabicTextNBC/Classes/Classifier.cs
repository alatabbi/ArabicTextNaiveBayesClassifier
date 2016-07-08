/*
 * 
 * Copyright(C) 2016 - All Rights Reserved 
 * Author: Ali Alatabbi.
 * Email: alialatabbi@gmail.com.
 * You may use, distribute and modify this code,  
 * provided that this copyright notice(s) and this permission notice appear 
 * in all copies of the Software and supporting documentation.
 * 
 */


using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace ATNBC
{
    
    public class NaiveBayesianClassifier
    {
        public string Path2ClassSet;
        List<Category> _Categories;
        int _NumberofTrainingRecords;
        int _NumberofUniqWordsCount;
        public NaiveBayesianClassifier(string path2ClassSet)
        {
            this.Path2ClassSet = path2ClassSet;
            this.Init();
        }
        private void Init()
        {
            _Categories = new List<Category>();
            (new DirectoryInfo(Path2ClassSet)).GetDirectories("*.*", SearchOption.TopDirectoryOnly).ToList().ForEach(di => {
                _Categories.Add(new Category(di.Name, di.GetFiles("*.txt", SearchOption.TopDirectoryOnly).Select(x => x.FullName).ToList()));
            });
            List<Record> trainingSet = _Categories.SelectMany(x => x.Records.Select(r => new Record(x.Name, r))).ToList();
            _NumberofTrainingRecords = trainingSet.Count;
            _NumberofUniqWordsCount = trainingSet.SelectMany(r => r.Text.ExtractWordsAr().ToArray()).GroupBy(w => w).Count();
        }

        public string GuessMostProbableClass(FileInfo fi)
        {
            string errMsg = "";
            string txt = Helper.ExtractText(fi.FullName, out errMsg);
            if (!string.IsNullOrEmpty(errMsg))
                return "";
            Dictionary<string, double> results = ComputeScoreMatrix(txt);
            if (results.OrderByDescending(x => x.Value).First().Value == 0)
                return "";
            return results.OrderByDescending(x => x.Value).First().Key;
        }

        public string GuessMostProbableClass(string txt)
        {
            Dictionary<string, double> results = ComputeScoreMatrix(txt);
            if (results.OrderByDescending(x => x.Value).First().Value == 0)
                return "";
            return results.OrderByDescending(x => x.Value).First().Key;
        }

        public Dictionary<string, double> ComputeScoreMatrix(string txt)
        {
            Dictionary<string, double> m = new Dictionary<string, double>();
            _Categories.ForEach(cat => m.Add(cat.Name, 0.0));
            
            foreach (string line in txt.ExtractLinesAr())
            {
                var words = line.ExtractWordsAr();
                _Categories.ForEach(c => {
                    m[c.Name] += Math.Pow(Math.E, c.CalculateScore(_NumberofTrainingRecords, _NumberofUniqWordsCount, words));
                });
            }
            return m;
        }
    }

    class Record
    {
        public Record(string className, string text)
        {
            Class = className;
            Text = text;
        }
        public string Class { get; set; }
        public string Text { get; set; }
    }

    class Category
    {
        private readonly List<string> TrainingDocuments;

        public List<string> Records
        {
            get; private set;
        }

        public Category(string name, List<String> trainingDocuments)
        {
            Records = new List<string>();
            this.Name = name;
            this.TrainingDocuments = trainingDocuments;
            this.Init();
        }

        private void Init()
        {
            string errMsg = "";

            foreach (string document in this.TrainingDocuments)
            {
                string text = Helper.ExtractText(document, out errMsg);
                if (!string.IsNullOrEmpty(errMsg))
                    continue;
                Records.AddRange(text.ExtractLinesAr());
            }

            this.NumberOfRecords = Records.Count;
            var features = Records.SelectMany(x => x.ExtractWordsAr());
            NumberofWords = features.Count();
            this.EachWordCount =
                  features.GroupBy(x => x)
                    .ToDictionary(x => x.Key, x => x.Count());
            this.NumberofUniqueWords = this.EachWordCount.Count;
        }

        public string Name { get; private set; }

        public int NumberofWords { get; private set; }

        public int NumberofUniqueWords { get; private set; }

        public Dictionary<string, int> EachWordCount { get; private set; }

        public int NumberOfRecords { get; private set; }

        public int NumberOfOccurencesInTrainingDocuments(String word)
        {
            if (EachWordCount.Keys.Contains(word))
                return EachWordCount[word];
            return 0;
        }

        public double CalculateScore(double trainingRecordsCount, double trainingUniqWordsCount, List<String> q)
        {
            return Math.Log(this.NumberOfRecords / trainingRecordsCount) + q.Sum(w => Math.Log((this.NumberOfOccurencesInTrainingDocuments(w) + 1) / (trainingUniqWordsCount + this.NumberofWords)));
        }


    }
}