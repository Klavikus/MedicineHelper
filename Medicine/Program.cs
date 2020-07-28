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
            Console.WriteLine(pred2.GetInfo());
            MedCourse preCourse2 = new MedCourse(
                pred2, 18,
                new Dictionary<float, float>() { { 8, 9 }, { 10, 5 }, { 11, 4 } },
                14,
                new int[] { 0 });

            CourseDistributer distrCourse2 = new CourseDistributer(preCourse2);
            distrCourse2.PrintInfo();
        }

        static void Main(string[] args)
        {
           TestMedFunctional();
        }
    }
}
