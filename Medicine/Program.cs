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

            Console.WriteLine(uriName);

            var url = $"https://www.rlsnet.ru/search_result.htm?word={uriName}";
           
            var req = GetHtml(url);
            
           // Console.WriteLine(req);

            bool reqOk = Regex.Match(req, $"<div class=\"search_page_head\">", RegexOptions.IgnoreCase).Success;


            var farmG = new List<string>();

            Console.WriteLine(reqOk);
            if (reqOk)
            {
                string regPattern1 = "в фармакологических группах([\\s\\S])*?(?=в нозологическом указателе)";
                Match farmGroups = Regex.Match(req, regPattern1, RegexOptions.IgnoreCase);
                
             //   Console.WriteLine(Regex.Unescape(req));
                if (farmGroups.Success)
                {
                    //Console.WriteLine(farmGroups.Groups[0].Value);
                    File.WriteAllText("reg.txt", farmGroups.Groups[0].Value);

                    var result = farmGroups.Groups[0].Value;
                    var regPattern2 = "(.htm\\\">)([\\s\\S]*?)(?=(<\\/a))";
                    Console.WriteLine(result);
                   
                    var matches = Regex.Matches(result, regPattern2, RegexOptions.IgnoreCase);
                    Console.WriteLine(matches.Count);
                    
                    foreach (Match m in matches)
                    {
                        farmG.Add(m.Groups[2].Value);
                    }

                   Console.WriteLine(string.Join("\n",farmG)); 
                }
            }

            //try
            //{
            //    var pathMedCatalog = @"med\";

            //    var KillList = File.ReadAllText(pathMedCatalog + "KillList.txt");
            //    var GoodList = File.ReadAllText(pathMedCatalog + "GoodList.txt");

            //    bool inKillList = Regex.Match(KillList, $"<li>{name}", RegexOptions.IgnoreCase).Success;
            //    bool inGoodList = Regex.Match(GoodList, $"{name}", RegexOptions.IgnoreCase).Success;


            //}
            //catch (Exception)
            //{

            //}

        }

        static void Main(string[] args)
        {
           //TestMedFunctional();
            MedTestByName("арбидол");
        }
    }
}
