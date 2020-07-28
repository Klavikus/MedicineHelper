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
        static void MedTestByName(string name)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding encoding = Encoding.GetEncoding("Windows-1251");

            var uriName = string.Join("", encoding.GetBytes(name).Select(c => Uri.HexEscape((char)c)));

            var url = $"https://www.rlsnet.ru/search_result.htm?word={uriName}";

            var req = GetHtml(url);

            bool reqOk = Regex.Match(req, $"<div class=\"search_page_head\">", RegexOptions.IgnoreCase).Success;

            var tradeNameGroup = new List<string>();
            var activeIngridients = new List<string>();
            var pharmaGroup = new List<string>();
            var nosological = new List<string>();


           
            if (reqOk)
            {
                var pharmaGroupPattern1 = "в фармакологических группах([\\s\\S])*?(?=в нозологическом указателе)";
                var pharmaGroupPattern2 = "(.htm\\\">)([\\s\\S]*?)(?=(<\\/a))";

                pharmaGroup = regexSearch(req, pharmaGroupPattern1, pharmaGroupPattern2);



               // var p = pharmaGroup.Select(x => x.Where(y=> Char.IsLetter(y)? y: ' '));
                Console.WriteLine(string.Join("\n", pharmaGroup));


            }
        }
        public static List<string> regexSearch(string context, string matchPattern, string matchesPattern)
        {

            var match = Regex.Match(context, matchPattern);
            var matchess = Regex.Matches(match.Groups[0].Value, matchesPattern);

            return matchess.Select(x => x.Groups[2].Value).ToList();
        }
        static void Main(string[] args)
        {
           //TestMedFunctional();
            MedTestByName("ибупрофен");
        }
    }
}
