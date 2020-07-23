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

/*
-55 = Âm năm mươi lăm
-1055 = Âm một nghìn không trăm năm mươi lăm
101002101000000000 = Một trăm lẻ một triệu tỷ không trăm lẻ hai nghìn tỷ một trăm lẻ một tỷ
100000000000 = Một trăm tỷ
1000000000000 = Một nghìn tỷ
1000000000000000 = Một triệu tỷ
1000000000000000000 = Một tỷ tỷ
1000000234000000000 = Một tỷ tỷ hai trăm ba mươi tư tỷ
1000000000222000000 = Một tỷ tỷ hai trăm hai mươi hai triệu
1009 = Một nghìn không trăm lẻ chín
9 = Chín
90 = Chín mươi
900 = Chín trăm
1000000 = Một triệu
1090000 = Một triệu không trăm chín mươi nghìn
1090010 = Một triệu không trăm chín mươi nghìn không trăm mười
1000010 = Một triệu không trăm mười
10 = Mười
100 = Một trăm
55 = Năm mươi lăm
20000 = Hai mươi nghìn
100000 = Một trăm nghìn
110000 = Một trăm mười nghìn
120000 = Một trăm hai mươi nghìn
130000 = Một trăm ba mươi nghìn
140000 = Một trăm bốn mươi nghìn
150000 = Một trăm năm mươi nghìn
160000 = Một trăm sáu mươi nghìn
170000 = Một trăm bảy mươi nghìn
180000 = Một trăm tám mươi nghìn
190000 = Một trăm chín mươi nghìn
200000 = Hai trăm nghìn
210000 = Hai trăm mười nghìn
220000 = Hai trăm hai mươi nghìn
230000 = Hai trăm ba mươi nghìn
240000 = Hai trăm bốn mươi nghìn
250000 = Hai trăm năm mươi nghìn
260000 = Hai trăm sáu mươi nghìn
270000 = Hai trăm bảy mươi nghìn
280000 = Hai trăm tám mươi nghìn
290000 = Hai trăm chín mươi nghìn
300000 = Ba trăm nghìn
310000 = Ba trăm mười nghìn
320000 = Ba trăm hai mươi nghìn
330000 = Ba trăm ba mươi nghìn
340000 = Ba trăm bốn mươi nghìn
350000 = Ba trăm năm mươi nghìn
360000 = Ba trăm sáu mươi nghìn
370000 = Ba trăm bảy mươi nghìn
380000 = Ba trăm tám mươi nghìn
390000 = Ba trăm chín mươi nghìn
400000 = Bốn trăm nghìn
410000 = Bốn trăm mười nghìn
420000 = Bốn trăm hai mươi nghìn
430000 = Bốn trăm ba mươi nghìn
440000 = Bốn trăm bốn mươi nghìn
450000 = Bốn trăm năm mươi nghìn
460000 = Bốn trăm sáu mươi nghìn
470000 = Bốn trăm bảy mươi nghìn
480000 = Bốn trăm tám mươi nghìn
490000 = Bốn trăm chín mươi nghìn
500000 = Năm trăm nghìn
510000 = Năm trăm mười nghìn
520000 = Năm trăm hai mươi nghìn
530000 = Năm trăm ba mươi nghìn
540000 = Năm trăm bốn mươi nghìn
550000 = Năm trăm năm mươi nghìn
560000 = Năm trăm sáu mươi nghìn
570000 = Năm trăm bảy mươi nghìn
580000 = Năm trăm tám mươi nghìn
590000 = Năm trăm chín mươi nghìn
600000 = Sáu trăm nghìn
610000 = Sáu trăm mười nghìn
620000 = Sáu trăm hai mươi nghìn
630000 = Sáu trăm ba mươi nghìn
640000 = Sáu trăm bốn mươi nghìn
650000 = Sáu trăm năm mươi nghìn
660000 = Sáu trăm sáu mươi nghìn
670000 = Sáu trăm bảy mươi nghìn
680000 = Sáu trăm tám mươi nghìn
690000 = Sáu trăm chín mươi nghìn
700000 = Bảy trăm nghìn
710000 = Bảy trăm mười nghìn
720000 = Bảy trăm hai mươi nghìn
730000 = Bảy trăm ba mươi nghìn
740000 = Bảy trăm bốn mươi nghìn
750000 = Bảy trăm năm mươi nghìn
760000 = Bảy trăm sáu mươi nghìn
770000 = Bảy trăm bảy mươi nghìn
780000 = Bảy trăm tám mươi nghìn
790000 = Bảy trăm chín mươi nghìn
800000 = Tám trăm nghìn
810000 = Tám trăm mười nghìn
820000 = Tám trăm hai mươi nghìn
830000 = Tám trăm ba mươi nghìn
840000 = Tám trăm bốn mươi nghìn
850000 = Tám trăm năm mươi nghìn
860000 = Tám trăm sáu mươi nghìn
870000 = Tám trăm bảy mươi nghìn
880000 = Tám trăm tám mươi nghìn
890000 = Tám trăm chín mươi nghìn
900000 = Chín trăm nghìn
910000 = Chín trăm mười nghìn
920000 = Chín trăm hai mươi nghìn
930000 = Chín trăm ba mươi nghìn
940000 = Chín trăm bốn mươi nghìn
950000 = Chín trăm năm mươi nghìn
960000 = Chín trăm sáu mươi nghìn
970000 = Chín trăm bảy mươi nghìn
980000 = Chín trăm tám mươi nghìn
990000 = Chín trăm chín mươi nghìn
1000000 = Một triệu
1010000 = Một triệu không trăm mười nghìn
1020000 = Một triệu không trăm hai mươi nghìn
1030000 = Một triệu không trăm ba mươi nghìn
1040000 = Một triệu không trăm bốn mươi nghìn
1050000 = Một triệu không trăm năm mươi nghìn
1060000 = Một triệu không trăm sáu mươi nghìn
1070000 = Một triệu không trăm bảy mươi nghìn
1080000 = Một triệu không trăm tám mươi nghìn
1090000 = Một triệu không trăm chín mươi nghìn
1100000 = Một triệu một trăm nghìn
1110000 = Một triệu một trăm mười nghìn
1120000 = Một triệu một trăm hai mươi nghìn
1130000 = Một triệu một trăm ba mươi nghìn
1140000 = Một triệu một trăm bốn mươi nghìn
1150000 = Một triệu một trăm năm mươi nghìn
1160000 = Một triệu một trăm sáu mươi nghìn
1170000 = Một triệu một trăm bảy mươi nghìn
1180000 = Một triệu một trăm tám mươi nghìn
1190000 = Một triệu một trăm chín mươi nghìn
1200000 = Một triệu hai trăm nghìn
1210000 = Một triệu hai trăm mười nghìn
1220000 = Một triệu hai trăm hai mươi nghìn
1230000 = Một triệu hai trăm ba mươi nghìn
1240000 = Một triệu hai trăm bốn mươi nghìn
1250000 = Một triệu hai trăm năm mươi nghìn
1260000 = Một triệu hai trăm sáu mươi nghìn
1270000 = Một triệu hai trăm bảy mươi nghìn
1280000 = Một triệu hai trăm tám mươi nghìn
1290000 = Một triệu hai trăm chín mươi nghìn
1300000 = Một triệu ba trăm nghìn
1310000 = Một triệu ba trăm mười nghìn
1320000 = Một triệu ba trăm hai mươi nghìn
1330000 = Một triệu ba trăm ba mươi nghìn
1340000 = Một triệu ba trăm bốn mươi nghìn
1350000 = Một triệu ba trăm năm mươi nghìn
1360000 = Một triệu ba trăm sáu mươi nghìn
1370000 = Một triệu ba trăm bảy mươi nghìn
1380000 = Một triệu ba trăm tám mươi nghìn
1390000 = Một triệu ba trăm chín mươi nghìn
1400000 = Một triệu bốn trăm nghìn
1410000 = Một triệu bốn trăm mười nghìn
1420000 = Một triệu bốn trăm hai mươi nghìn
1430000 = Một triệu bốn trăm ba mươi nghìn
1440000 = Một triệu bốn trăm bốn mươi nghìn
1450000 = Một triệu bốn trăm năm mươi nghìn
1460000 = Một triệu bốn trăm sáu mươi nghìn
1470000 = Một triệu bốn trăm bảy mươi nghìn
1480000 = Một triệu bốn trăm tám mươi nghìn
1490000 = Một triệu bốn trăm chín mươi nghìn
1500000 = Một triệu năm trăm nghìn
1510000 = Một triệu năm trăm mười nghìn
1520000 = Một triệu năm trăm hai mươi nghìn
1530000 = Một triệu năm trăm ba mươi nghìn
1540000 = Một triệu năm trăm bốn mươi nghìn
1550000 = Một triệu năm trăm năm mươi nghìn
1560000 = Một triệu năm trăm sáu mươi nghìn
1570000 = Một triệu năm trăm bảy mươi nghìn
1580000 = Một triệu năm trăm tám mươi nghìn
1590000 = Một triệu năm trăm chín mươi nghìn
1600000 = Một triệu sáu trăm nghìn
1610000 = Một triệu sáu trăm mười nghìn
1620000 = Một triệu sáu trăm hai mươi nghìn
1630000 = Một triệu sáu trăm ba mươi nghìn
1640000 = Một triệu sáu trăm bốn mươi nghìn
1650000 = Một triệu sáu trăm năm mươi nghìn
1660000 = Một triệu sáu trăm sáu mươi nghìn
1670000 = Một triệu sáu trăm bảy mươi nghìn
1680000 = Một triệu sáu trăm tám mươi nghìn
1690000 = Một triệu sáu trăm chín mươi nghìn
1700000 = Một triệu bảy trăm nghìn
1710000 = Một triệu bảy trăm mười nghìn
1720000 = Một triệu bảy trăm hai mươi nghìn
1730000 = Một triệu bảy trăm ba mươi nghìn
1740000 = Một triệu bảy trăm bốn mươi nghìn
1750000 = Một triệu bảy trăm năm mươi nghìn
1760000 = Một triệu bảy trăm sáu mươi nghìn
1770000 = Một triệu bảy trăm bảy mươi nghìn
1780000 = Một triệu bảy trăm tám mươi nghìn
1790000 = Một triệu bảy trăm chín mươi nghìn
1800000 = Một triệu tám trăm nghìn
1810000 = Một triệu tám trăm mười nghìn
1820000 = Một triệu tám trăm hai mươi nghìn
1830000 = Một triệu tám trăm ba mươi nghìn
1840000 = Một triệu tám trăm bốn mươi nghìn
1850000 = Một triệu tám trăm năm mươi nghìn
1860000 = Một triệu tám trăm sáu mươi nghìn
1870000 = Một triệu tám trăm bảy mươi nghìn
1880000 = Một triệu tám trăm tám mươi nghìn
1890000 = Một triệu tám trăm chín mươi nghìn
1900000 = Một triệu chín trăm nghìn
1910000 = Một triệu chín trăm mười nghìn
1920000 = Một triệu chín trăm hai mươi nghìn
1930000 = Một triệu chín trăm ba mươi nghìn
1940000 = Một triệu chín trăm bốn mươi nghìn
1950000 = Một triệu chín trăm năm mươi nghìn
1960000 = Một triệu chín trăm sáu mươi nghìn
1970000 = Một triệu chín trăm bảy mươi nghìn
1980000 = Một triệu chín trăm tám mươi nghìn
1990000 = Một triệu chín trăm chín mươi nghìn
2000000 = Hai triệu
*/
