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
            Med med = new Med();

            med.Name = "оциллококцинум";
            med.MinDoseCapacity = 30;
            med.DoseUnit = "таб";
            med.MinDose = 1;
            med.Price = 120;
            med.GetMedInfo();

            Console.WriteLine(med.GetInfo());

            MedCourse medCourse = new MedCourse();

            medCourse.Medicine = med;
            medCourse.DayDose = 18;
            medCourse.DayDoseDistr = new Dictionary<double, double>() { { 8, 9 }, { 10, 5 }, { 11, 4 } };
            medCourse.CourseDayInterval = 14;
            medCourse.CourseDayPattern = new int[] { 1 };
            medCourse.CalculateCourse();

            CourseDistributer distributedCourse = new CourseDistributer(medCourse);
            distributedCourse.startCourseDay = DateTime.Now;
            distributedCourse.DistributeCourseByData();
            distributedCourse.PrintInfo();


            for (int i = 0; i < 14; i++)
            {
                distributedCourse.GetShedule(DateTime.Now.AddDays(i));
            }
            
        }

        static void Main(string[] args)
        {
           TestMedFunctional();
        }
    }
}
