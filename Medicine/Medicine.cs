using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Medicine
{
    /// <summary>
    /// Дополнительные методы работы с препаратом.
    /// </summary>
    public class MedHelper
    {
        
    }

    /// <summary>
    /// Отслеживание курса препарата
    /// </summary>
    public class CourseDistributer
    {
        public bool[] MedStatus { get; private set; }
        public MedCourse Course { get; private set;}
        public CourseDistributer(MedCourse medCourse)
        {
            Course = medCourse;
            MedStatus = TestName(medCourse.Medicine.Name);
        }
        public void PrintInfo()
        {
            Console.WriteLine($"Курс препарата \"{Course.Medicine.Name}\"");
            Console.WriteLine($"Нахождение в расстрельном списке: {MedStatus[0]}");
            Console.WriteLine($"Нахождение в списке жизненно необходимых: {MedStatus[1]}");
            Console.WriteLine($"Расписание составленно на {Course.CourseDayInterval}");

            foreach (var distribution in Course.CourseDayDistribution)
            {
                Console.Write($"{distribution.Key} день ");
                foreach (var day in distribution.Value)
                {
                    Console.Write($"в {day.Key} ч. приём - {day.Value} ");
                }
                Console.WriteLine();
            }

        }

        /// <summary>
        /// Проверяет вхождения препарата в расстрельный и жизненно необходимый списки препаратов.
        /// </summary>
        /// <param name="name">Название препарата.</param>
        /// <returns>bool[0] - фуфломицин, bool[1] - жизненно необходим</returns>
        public static bool[] TestName(string name)
        {
            try
            {
                var pathMedCatalog = @"med\";

                var KillList = File.ReadAllText(pathMedCatalog + "KillList.txt");
                var GoodList = File.ReadAllText(pathMedCatalog + "GoodList.txt");

                bool inKillList = Regex.Match(KillList, $"<li>{name}", RegexOptions.IgnoreCase).Success;
                bool inGoodList = Regex.Match(GoodList, $"{name}", RegexOptions.IgnoreCase).Success;

                return new bool[] { inKillList, inGoodList };
            }
            catch (Exception)
            {
                return new bool[] { false, false };
            }
        }

    }

    /// <summary>
    /// Курс препарата
    /// </summary>
    public class MedCourse
    {
        public Med Medicine { get; private set; }
        public float DayDose { get; private set; }
        public Dictionary<float, float> DayDoseDistr { get; private set; }
        public Dictionary<float, float> Dose { get; private set; }
        public int CourseDayInterval { get; private set; }
        public int[] CourseDayPattern { get; private set; }
        public Dictionary<int, Dictionary<float, float>> CourseDayDistribution { get; private set; }
        public MedCourse(Med medicine, 
                         float dayDose, 
                         Dictionary<float, float> dayDoseDistr, 
                         int courseDayInterval, 
                         int[] courseDayPattern)
        {
            this.Medicine = medicine;
            this.DayDose = dayDose;
            this.DayDoseDistr = dayDoseDistr;
            this.CourseDayInterval = courseDayInterval;
            this.CourseDayPattern = courseDayPattern;
            this.CourseDayDistribution = new Dictionary<int, Dictionary<float, float>>();
            this.CalculateDayDose();
            this.DistribDoseByDay();
        }

        /// <summary>
        /// Возвращает массив дней приёма препарата.
        /// </summary>        
        /// <param name="seq">последовательность приёма препарата</param>
        /// <param name="dayCount">длительность курса</param>
        private int[] DistribSeqByDay()
        {   
            
            //Если последователшность распределения включает несколько элеметнов
            //то она и является последовательностью приёма препарата
            if (this.CourseDayPattern.Length > 1) return this.CourseDayPattern;

            //В ином случае, входная последовательность является приращением
            var result = new List<int>() { 1 };
            var dayInCourse = CourseDayInterval / (this.CourseDayPattern[0] + 1);
            var i = 1;

            while (result.Count <= dayInCourse)
            {
                result.Add(result[i - 1] + this.CourseDayPattern[0] + 1);
                i++;
            }

            if (result[result.Count-1] > CourseDayInterval) result.RemoveAt(result.Count - 1);

            return result.ToArray();
        }
        private void DistribDoseByDay()
        {
            //Console.WriteLine(string.Join(" ", DistribSeqByDay()));
            foreach (var day in DistribSeqByDay())
                this.CourseDayDistribution.Add(day, this.Dose);
        }
        private void CalculateDayDose()
        {
            var tempDict = new Dictionary<float, float>();
            foreach (var dose in DayDoseDistr)
            {
                tempDict.Add(dose.Key, dose.Value);
            }
            Dose = tempDict;
        }

    }

    /// <summary>
    /// Препарат
    /// </summary>
    public class Med
    {
        /// <summary>
        /// Название препарата.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Стоимость препарата в рублях.
        /// </summary>
        public float Price{ get; private set; }
        /// <summary>
        /// Единицы дозирования (мг/таб/т.д.)
        /// </summary>
        public string DoseUnit { get; private set; }
        /// <summary>
        /// Минимальная расфасованная доза.
        /// </summary>
        public float MinDose{ get; private set; }
        /// <summary>
        /// Ёмкость упаковки в минимальных дозах.
        /// </summary>
        public float MinDoseCapacity{ get; private set; }

        /// <summary>
        /// Торговое наименование препарата.
        /// </summary>
        public List<string> TradeNameGroup { get; private set; }

        /// <summary>
        /// Действующие вещества, в составе препарата.
        /// </summary>
        public List<string> ActiveIngridients { get; private set; }

        /// <summary>
        /// Фармокологическая группа
        /// </summary>
        public List<string> PharmaGroup { get; private set; }

        /// <summary>
        /// Нозологическая классификация.
        /// </summary>
        public List<string> Nosological { get; private set; }

        /// <summary>
        /// Медикамент
        /// </summary>
        /// <param name="name">Наименование препарата.</param>
        /// <param name="price">Стоимость перпарата в рублях.</param>
        /// <param name="doseUnit">Единицы дозирования (мг/таб/т.д.)</param>
        /// <param name="minDose">Минимальная расфасованная доза.</param>
        /// <param name="minDoseCapacity">Ёмкость упаковки в минимальных дозах.</param>
        public Med(string name, float price, string doseUnit, float minDose, float minDoseCapacity)
        {
            Name = name;
            Price = price;
            DoseUnit = doseUnit;
            MinDose = minDose;
            MinDoseCapacity = minDoseCapacity;

            TradeNameGroup = new List<string>();
            ActiveIngridients = new List<string>();
            PharmaGroup = new List<string>();
            Nosological = new List<string>();
            GetMedInfo();
        }

        /// <summary>
        /// Возвращает сайт в виде строки, кодировка win-1251
        /// </summary>
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

        /// <summary>
        /// Вспомогательный метод парсинга, рассчитанный на сайт www.rlsnet.ru.
        /// </summary>
        static List<string> RlsnetSearch(string context, string matchPattern)
        {
            var matchesPattern = "(.htm\\\">)([\\s\\S]*?)(?=(<\\/a))";
            var match = Regex.Match(context, matchPattern);
            var matchess = Regex.Matches(match.Groups[0].Value, matchesPattern);
            return matchess.Select(x => WebUtility.HtmlDecode(Regex.Replace(x.Groups[2].Value, "<[^>]*(>|$)", " "))).ToList();
        }

        /// <summary>
        /// Собирает основные данные препарата, используя сайт www.rlsnet.ru.
        /// </summary
        void GetMedInfo()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding encoding = Encoding.GetEncoding("Windows-1251");

            var uriName = string.Join("", encoding.GetBytes(Name).Select(c => Uri.HexEscape((char)c)));

            var url = $"https://www.rlsnet.ru/search_result.htm?word={uriName}";

            var reqGeneral = GetHtml(url);

            bool reqOk = Regex.Match(reqGeneral, $"<div class=\"search_page_head\">").Success;

            if (reqOk)
            {
                var pharmaGroupPattern = "в фармакологических группах([\\s\\S])*?(?=в нозологическом указателе)";
                var activeIngridientsPattern = "в действующих веществах([\\s\\S])*?(?=в фармакологических группах)";
                var tradeNameGroupPattern = "в торговых названиях([\\s\\S])*?(style)";

                PharmaGroup = RlsnetSearch(reqGeneral, pharmaGroupPattern);
                ActiveIngridients = RlsnetSearch(reqGeneral, activeIngridientsPattern);
                TradeNameGroup = RlsnetSearch(reqGeneral, tradeNameGroupPattern);

                var nosologicalPattern0 = "(в торговых названиях)[\\s\\S]*?href=\"\\/\\/([\\s\\S]*?)(\\\">)";
                var nosologicalPattern1 = "(Нозологическая классификация)[\\s\\S]*?(<div)";

                var reqMed = GetHtml($"https://{Regex.Match(reqGeneral, nosologicalPattern0).Groups[2].Value}");

                Nosological = RlsnetSearch(reqMed, nosologicalPattern1);
            }
        }

        /// <summary>
        /// Общая сводка о препарате.
        /// </summary>
        /// <returns></returns>
        public string GetInfo()
        {
            string info =
                $"Наименование: {Name}\n" +
                $"Цена: {Price} руб\n" +
                $"Минимальная доза: {MinDose} {DoseUnit}\n" +
                $"Ёмкость: {MinDoseCapacity} {DoseUnit}\n" +
                $"\nТорговые названия:\n{string.Join("\n", TradeNameGroup)}" +
                $"\nФармокологические группы:\n{string.Join("\n", PharmaGroup)}" +
                $"\nДействующие вещества:\n{string.Join("\n", ActiveIngridients)}" +
                $"\nНозологическая классификация:\n{string.Join("\n", Nosological)}";

            return info;
        }
    }
}
