using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace Medicine
{
    class Program
    {
        static void TestMedFunctional()
        {
            Med pred2 = new Med("оциллококцинум", 30, "таб", 1, 120);
            MedCourse preCourse2 = new MedCourse(
                pred2, 18,
                new Dictionary<float, float>() { { 8, 9 }, { 10, 5 }, { 11, 4 } },
                14,
                new int[] { 0 });

            CourseDistributer distrCourse2 = new CourseDistributer(preCourse2);
            distrCourse2.PrintInfo();
        }
        static string GetHtml(string url)
        {
            try
            {
                WebClient wb = new WebClient();
                wb.Encoding = Encoding.GetEncoding("Windows-1251");
                return wb.DownloadString(url);
            }
            catch (Exception)
            {

            }
            return "";
        }
        static void MedInfoByName(string name)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding encoding = Encoding.GetEncoding("Windows-1251");

            var uriName = string.Join("", encoding.GetBytes(name).Select(c => Uri.HexEscape((char)c)));

            var url = $"https://www.rlsnet.ru/search_result.htm?word={uriName}";

            var reqGeneral = GetHtml(url);

            var tradeNameGroup = new List<string>();
            var activeIngridients = new List<string>();
            var pharmaGroup = new List<string>();
            var nosological = new List<string>();

            bool reqOk = Regex.Match(reqGeneral, $"<div class=\"search_page_head\">").Success;

            if (reqOk)
            {
                var pharmaGroupPattern = "в фармакологических группах([\\s\\S])*?(?=в нозологическом указателе)";
                var activeIngridientsPattern = "в действующих веществах([\\s\\S])*?(?=в фармакологических группах)";
                var tradeNameGroupPattern = "в торговых названиях([\\s\\S])*?(style)";
               
                pharmaGroup = regexSearch(reqGeneral, pharmaGroupPattern);
                activeIngridients = regexSearch(reqGeneral, activeIngridientsPattern);
                tradeNameGroup = regexSearch(reqGeneral, tradeNameGroupPattern);

                var nosologicalPattern0 = "(в торговых названиях)[\\s\\S]*?href=\"\\/\\/([\\s\\S]*?)(\\\">)";
                var nosologicalPattern1 = "(Нозологическая классификация)[\\s\\S]*?(<div)";
                var reqMed = GetHtml($"https://{Regex.Match(reqGeneral, nosologicalPattern0).Groups[2].Value}");

                nosological = regexSearch(reqMed, nosologicalPattern1);

                Console.WriteLine("\nТорговые названия:\n" + string.Join("\n", tradeNameGroup));
                Console.WriteLine("\nФармокологические группы:\n" + string.Join("\n", pharmaGroup));
                Console.WriteLine("\nДействующие вещества:\n" + string.Join("\n", activeIngridients));
                Console.WriteLine("\nНозологическая классификация:\n" + string.Join("\n", nosological));
            }
        }
        static List<string> regexSearch(string context, string matchPattern)
        {
            var matchesPattern = "(.htm\\\">)([\\s\\S]*?)(?=(<\\/a))";
            var match = Regex.Match(context, matchPattern);
            var matchess = Regex.Matches(match.Groups[0].Value, matchesPattern);
            return matchess.Select(x => WebUtility.HtmlDecode(Regex.Replace(x.Groups[2].Value, "<[^>]*(>|$)", " "))).ToList();
        }
        static void Main(string[] args)
        {
            //TestMedFunctional();
            MedInfoByName("оциллококцинум");
        }
    }
}
