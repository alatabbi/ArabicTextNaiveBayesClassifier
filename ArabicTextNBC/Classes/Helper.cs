﻿/*
The MIT License (MIT)

Copyright(C) 2016 - All Rights Reserved.
Author: Ali Alatabbi.
Email: alialatabbi@gmail.com.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ATNBC
{
    public static class Helper
    {

        public static readonly Dictionary<string, string> Alerts= new Dictionary<string, string>
        {
            {"Start", "<div class=\"alert alert-info\" role=\"alert\"> Start</div>" },
            {"End", "<div class=\"alert alert-info\" role=\"alert\"> End</div> " },
            {"Cancel", "<div class=\"alert alert-warning\" role=\"alert\"><strong>Canceled</strong></div>" },
            {"Error", "<div class=\"alert alert-warning\" role=\"warning\"><strong>Erro</strong></div>" }
        };
 
         public static string ExtractText(string fileName, out string progMsg)
        {
            progMsg = "";
            try
            {
                Encoding ENC = Encoding.UTF8;
                FileInfo ff = new FileInfo(fileName);
                if (!ff.Exists)
                {
                    progMsg = "File does not exists!";
                    return "";
                }
                string txt = "";
                if (ff.Extension == ".txt")
                {
                    txt = File.ReadAllText(fileName, ENC);
                }


                txt = txt.Replace((char)0x00, ' ').Trim();
                if (string.IsNullOrEmpty(txt.Trim()))
                    progMsg = "Error: Empty file!";
                return txt;
            }
            catch (System.Exception ex)
            {
                progMsg = ex.Message;
                return string.Empty;
            }
        }
        public static List<String> ExtractWordsAr(this String text)
        {
            return Regex.Replace(text.NormaliseText(), "\\p{P}+", " ").Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public static List<String> ExtractLinesAr(this String text)
        {
            text = Regex.Replace(text.NormaliseText(), @"\d", " ");
            text = Regex.Replace(text, @"\s+", " ");
            text = Regex.Replace(text, @"[\p{P}]", Environment.NewLine);
            text = Regex.Replace(text, @"[\P{IsArabic}-[\p{P}\r\n]]", " ");
            List<String> list = new List<string>();
            text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(item => {

                if (!string.IsNullOrEmpty(item.Trim()))
                    list.Add(item.Trim());
            });
            return list;
        }

    } 
    public static class Ext
    {
        public static string ReplaceEnd(this string s, string s1, string s2)
        {
            if (!s.Contains(" "))
            {
                if (s.EndsWith(s1) && !s.StartsWith(s1))
                    s = s.Replace(s1, s2);
                return s;
            }
            else
            {
                string ret = "";
                s.Split(" ".ToCharArray()).ToList().ForEach(x => {
                    if (x.EndsWith(s1) && !x.StartsWith(s1))
                        ret += x.Replace(s1, s2);
                    else
                        ret += x + " ";
                });
                return ret.Trim();
            }
        }
        public static string ReplaceStartsWith(this string s, string s1, string s2)
        {
            if (s.StartsWith(s1))
                s = s.Replace(s1, s2);
            return s;

        }
        public static string ReplaceMany(this string s, string oldVal, string newVal)
        {
            return String.Join(newVal, s.Split(oldVal.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
        }
        public static bool ArabicContains(this string s1, string s2)
        {
            return s1.NormaliseText().Contains(s2.NormaliseText());
        }
        public static string NormaliseText(this string t)
        {
            t = Regex.Replace(t, @"ـ", @"");
            t = Regex.Replace(t, @"\p{M}", @"");
            t = Regex.Replace(t, @"[أإآءؤئ]", @"ا");
            t = t.Replace("ى", "ي");
            t = t.Replace("يي", "ي");
            t = t.Replace("ة", "ه");
            t = Regex.Replace(t, @"وا\b", @"و");
            t = Regex.Replace(t, @"اا", "ا");
            t = Regex.Replace(t, @"\b\w{1,2}\b", " ");

            t = Regex.Replace(t, @"\s+", " ");
            return t.Trim();
        }




    }
}