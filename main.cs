// C# 8

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NumberToVietnamese
{
    public static class NumberToVietnamese
    {
        private static readonly string[] ZeroLeftPadding = {"", "00", "0"};

        private static readonly string[] Digits =
        {
            "không",
            "một",
            "hai",
            "ba",
            "bốn",
            "năm",
            "sáu",
            "bảy",
            "tám",
            "chín"
        };

        private static readonly string[] MultipleThousand =
        {
            "",
            "nghìn",
            "triệu",
            "tỷ",
            "nghìn tỷ",
            "triệu tỷ",
            "tỷ tỷ"
        };

        private static IEnumerable<string> Chunked(this string str, int chunkSize) => Enumerable
            .Range(0, str.Length / chunkSize)
            .Select(i => str.Substring(i * chunkSize, chunkSize));

        private static bool ShouldShowZeroHundred(this string[] groups) =>
            groups.Reverse().TakeWhile(it => it == "000").Count() < groups.Count() - 1;

        private static void Deconstruct<T>(this IReadOnlyList<T> items, out T t0, out T t1, out T t2)
        {
            t0 = items.Count > 0 ? items[0] : default;
            t1 = items.Count > 1 ? items[1] : default;
            t2 = items.Count > 2 ? items[2] : default;
        }

        private static string ReadTriple(string triple, bool showZeroHundred)
        {
            var (a, b, c) = triple.Select(ch => int.Parse(ch.ToString())).ToArray();

            return a switch
            {
                0 when b == 0 && c == 0 => "",
                0 when showZeroHundred => "không trăm " + ReadPair(b, c),
                0 when b == 0 => Digits[c],
                0 => ReadPair(b, c),
                _ => Digits[a] + " trăm " + ReadPair(b, c)
            };
        }

        private static string ReadPair(int b, int c)
        {
            return b switch
            {
                0 => c == 0 ? "" : " lẻ " + Digits[c],
                1 => "mười " + c switch
                {
                    0 => "",
                    5 => "lăm",
                    _ => Digits[c]
                },
                _ => Digits[b] + " mươi " + c switch
                {
                    0 => "",
                    1 => "mốt",
                    4 => "tư",
                    5 => "lăm",
                    _ => Digits[c]
                }
            };
        }

        private static string Capitalize(this string input)
        {
            return input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => input.First().ToString().ToUpper() + input.Substring(1).ToLower()
            };
        }

        public static string ToVietnameseWords(this long n)
        {
            if (n == 0L) return "Không";
            if (n < 0L) return "Âm " + (-n).ToVietnameseWords().ToLower();

            var s = n.ToString();
            var groups = (ZeroLeftPadding[s.Length % 3] + s).Chunked(3).ToArray();
            var showZeroHundred = groups.ShouldShowZeroHundred();

            var index = -1;
            var rawResult = groups.Aggregate("", (acc, e) =>
            {
                checked
                {
                    index++;
                }

                var readTriple = ReadTriple(e, showZeroHundred && index > 0);
                var multipleThousand = (string.IsNullOrWhiteSpace(readTriple)
                    ? ""
                    : (MultipleThousand.ElementAtOrDefault(groups.Length - 1 - index) ?? ""));
                return $"{acc} {readTriple} {multipleThousand} ";
            });

            return Regex
                .Replace(rawResult, "\\s+", " ")
                .Trim()
                .Capitalize();
        }
    }

    internal static class MainClass
    {
        public static void Main(string[] args)
        {
            var numbers = new List<long>()
            {
                -55,
                -1055,
                101002101000000000,
                100000000000,
                1000000000000,
                1000000000000000,
                1000000000000000000,
                1000000234000000000,
                1000000000222000000,
                1009,
                9,
                90,
                900,
                1000000,
                1090000,
                1090010,
                1000010,
                10,
                100,
                55,
                20000
            };
            for (var i = 100000L; i <= 2000000; i += 10000L)
            {
                numbers.Add(i);
            }

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            foreach (var n in numbers)
            {
                Console.WriteLine("{0} = {1}", n, n.ToVietnameseWords());
            }
        }
    }
}
