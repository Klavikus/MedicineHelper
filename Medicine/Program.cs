using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Medicine.bot;

namespace Medicine
{
    class Program
    {
        public static void SaveSheduleSQL(CourseDistributer courseDistr)
        {
            using (var context = new MyDbContext())
            {
                foreach (var item in context.Shedule)
                {
                    context.Shedule.Remove(item);
                }
                context.SaveChanges();

                var sheduleList = new List<Shedule>();
                
                foreach (var data in courseDistr.CourseByDateTime)
                {
                    foreach (var dose in data.Value)
                    {
                        sheduleList.Add(new Shedule()
                        {
                            UserId = 99999,
                            CourseId = courseDistr.Guid,
                            MedId = courseDistr.Course.Medicine.Guid,
                            Date = data.Key,
                            Dose = dose.Value,
                            Status = false
                        });
                    }
                }

                context.Shedule.AddRange(sheduleList);

                context.SaveChanges();

            }
        }
        static void TestMedFunctional()
        {
            Med med = new Med();

            med.Name = "оциллококцинум";
            med.Guid = Guid.NewGuid();
            med.MinDoseCapacity = 30;
            med.DoseUnit = "таб";
            med.MinDose = 1;
            med.Price = 120;
            med.GetMedInfo();

          //  Console.WriteLine(med.GetInfo());

            MedCourse medCourse = new MedCourse();

            medCourse.Medicine = med;
            medCourse.Guid = Guid.NewGuid();
            medCourse.DayDose = 18;
            medCourse.DayDoseDistr = new Dictionary<double, double>() { { 8, 9 }, { 10, 5 }, { 11, 4 } };
            medCourse.CourseDayInterval = 14;
            medCourse.CourseDayPattern = new int[] { 1 };
            medCourse.CalculateCourse();

            CourseDistributer distributedCourse = new CourseDistributer(medCourse);
            distributedCourse.Guid = Guid.NewGuid();
            distributedCourse.startCourseDay = DateTime.Now;
            distributedCourse.DistributeCourseByData();
            distributedCourse.PrintInfo();

            var a = distributedCourse.CourseByDateTime;
            for (int i = 0; i < 14; i++)
            {
                distributedCourse.GetShedule(DateTime.Now.AddDays(i));
            }

            SaveSheduleSQL(distributedCourse);

        }

        static void Main(string[] args)
        {
            var mBot = new MedBot();
            mBot.Start();
            while (true)
            {

            }
  

          
           // TestMedFunctional();
        }
    }
}
