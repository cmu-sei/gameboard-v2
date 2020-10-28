// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Gameboard.Data
{
    public static class StringExtensions
    {
        internal const string DefaultUrlStringValue = "_";
        internal const string UrlDelimiter = "-";
        class Replacement
        {
            public Replacement()
            {
                Bad = new List<string>();
            }

            public Replacement(string bad, string good)
                : this()
            {
                Bad.Add(bad);
                Good = good;
            }

            public List<string> Bad { get; set; }
            public string Good { get; set; }
        }

        static readonly List<Replacement> Replacements = new List<Replacement>
        {
            new Replacement("'", ""),
            new Replacement { Bad = new List<string>{ "�", "�", "�", "�", "�" }, Good = "o" },
            new Replacement { Bad = new List<string>{ "�", "�" }, Good = "e" },
            new Replacement("�", "a"),
            new Replacement("�", "n"),
            new Replacement("�", "u"),
            new Replacement { Bad = new List<string>{ UrlDelimiter, "_", ".", "/", ":" }, Good = " " }
        };

        public static string ToUrlString(this string value, bool truncate = true, int truncateAt = 100)
        {
            if (string.IsNullOrEmpty(value))
                return DefaultUrlStringValue;

            string v = truncate ? value.Truncate(truncateAt) : value;

            foreach (var replacement in Replacements)
            {
                foreach (var bad in replacement.Bad)
                {
                    v = v.Replace(bad, replacement.Good);
                }
            }

            string result = Regex.Replace(v, @"[^a-zA-Z0-9\s]", "")
                .Replace(" ", UrlDelimiter)
                .Replace(UrlDelimiter + UrlDelimiter + UrlDelimiter, UrlDelimiter)
                .Replace(UrlDelimiter + UrlDelimiter, UrlDelimiter)
                .Trim(UrlDelimiter.ToCharArray());

            return string.IsNullOrEmpty(result) ? DefaultUrlStringValue : result.ToLower();
        }
        public static string Truncate(this string value, int max)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            string v = value.Trim();

            if (v.Length <= max)
                return v;

            return v.Substring(0, max - 3) + "...";
        }
    }
}

