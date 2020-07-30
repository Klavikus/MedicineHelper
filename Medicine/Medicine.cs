using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Medicine
{
    //TODO: Реализовать проверку корректности ввода информации.
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
        public Guid Guid { get; set; }
        /// <summary>
        /// Курс препарата.
        /// </summary>
        public MedCourse Course { get; private set; }

        /// <summary>
        /// Статус нахождения в расстрельном списке и списке жизненно необходимых препаратов.
        /// </summary>
        public bool[] MedStatus { get; private set; }

        /// <summary>
        /// Дата начала курса.
        /// </summary>
        public DateTime startCourseDay { get; set; }

        /// <summary>
        /// Распределения курса, по конкретным датам.
        /// </summary>
        public Dictionary<DateTime, Dictionary<double, double>> CourseByDateTime { get; set; }

        /// <summary>
        /// Конструктор распределения курса.
        /// </summary>
        /// <param name="medCourse">Курс препарата.</param>
        public CourseDistributer(MedCourse medCourse)
        {
            Course = medCourse;
            MedStatus = TestName(medCourse.Medicine.Name);
        }

        /// <summary>
        /// Тестовый метод вывода основной информации, по сформированному распределению курса.
        /// </summary>
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

        /// <summary>
        /// Распределяет курс препарата, в соответствии со стартовой датой.
        /// </summary>
        public void DistributeCourseByData()
        {
            //TODO: Упростить через итератор.
            var tempRes = new Dictionary<DateTime, Dictionary<double, double>>();
            var data = new DateTime(startCourseDay.Year, startCourseDay.Month, startCourseDay.Day);
            foreach (var day in Course.CourseDayDistribution)
                tempRes.Add(data.AddDays(day.Key - 1), day.Value);
            CourseByDateTime = tempRes;
        }

        /// <summary>
        /// Возвращает расписание приёма препарата в заданный день.
        /// </summary>
        public Dictionary<DateTime, double> GetShedule(DateTime day)
        {
            //TODO: Упростить преобразование даты
            var result = new DateTime(day.Year, day.Month, day.Day);
            var resDict = new Dictionary<DateTime, double>();

            if (CourseByDateTime.ContainsKey(result))
            {
                var d = CourseByDateTime[result];
                foreach (var period in d)
                {
                    resDict.Add(result.AddHours(period.Key), period.Value);
                }
            }

            foreach (var item in resDict)
            {
                Console.WriteLine($"Время приёма: {item.Key}  кол-во препарата: {item.Value}");
            }

            return resDict;
        }
    }

    /// <summary>
    /// Курс препарата
    /// </summary>
    public class MedCourse
    {
        public Guid Guid { get;  set; }
        /// <summary>
        /// Шаг отмены препарата в соответствии с дневным распределением.
        /// </summary>
        public double[] DayDoseDeltaDistr { get; set; }

        /// <summary>
        /// Препарат.
        /// </summary>
        public Med Medicine { get; set; }

        /// <summary>
        /// Необходимая дневная доза препарата.
        /// </summary>
        public float DayDose { get; set; }

        /// <summary>
        /// Распределение дозы в течении суток ("Время приёма в часах":"Кол-во минимальных дозах")
        /// </summary>
        public Dictionary<double, double> DayDoseDistr { get; set; }

        ///// <summary>
        ///// Распределение дозы в течении суток ("Время приёма в часах":"Кол-во в минимальных дозах")
        ///// </summary>
        //public Dictionary<double, double> Dose { get; set; }

        /// <summary>
        /// Длительность курса в днях.
        /// </summary>
        public int CourseDayInterval { get; set; }

        /// <summary>
        /// Распределение приёма по дням, если int[].length > 1, то и я вляется CourseDayDistribution
        /// </summary>
        public int[] CourseDayPattern { get; set; }

        /// <summary>
        /// Распределение дневных доз, по рассчитаным дням приёма.
        /// </summary>
        public Dictionary<int, Dictionary<double, double>> CourseDayDistribution { get; private set; }

        /// <summary>
        /// Возвращает массив дней приёма препарата.
        /// </summary>        
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

        /// <summary>
        /// формирование распределения дневных доз, по рассчитаным дням приёма.
        /// </summary>
        private void DistribDoseByDay()
        {
            CourseDayDistribution = new Dictionary<int, Dictionary<double, double>>();
            foreach (var day in DistribSeqByDay())
               CourseDayDistribution.Add(day, this.DayDoseDistr);
        }

        private void CalculateCancelDose()
        {
            // TODO: Реализовать отмену препарата.
        }


        /// <summary>
        /// Завершение конструирования модели курса.
        /// </summary>
        public void CalculateCourse()
        {
            this.DistribDoseByDay();
        }
    }

    /// <summary>
    /// Препарат
    /// </summary>
    public class Med
    {
        public Guid Guid { get; set; }
        /// <summary>
        /// Название препарата.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Стоимость препарата в рублях.
        /// </summary>
        public double Price{ get; set; }
        /// <summary>
        /// Единицы дозирования (мг/таб/т.д.)
        /// </summary>
        public string DoseUnit { get; set; }
        /// <summary>
        /// Минимальная расфасованная доза.
        /// </summary>
        public double MinDose { get; set; }
        /// <summary>
        /// Ёмкость упаковки в минимальных дозах.
        /// </summary>
        public double MinDoseCapacity { get; set; }

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
        public void GetMedInfo()
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
