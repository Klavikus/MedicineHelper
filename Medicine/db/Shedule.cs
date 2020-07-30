using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medicine
{
    public class Shedule
    {
        [Index(IsUnique = true)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public Guid CourseId { get; set; }
        public Guid MedId { get; set; }
        public DateTime Date { get; set; }
        public double Dose { get; set; }
        public bool Status { get; set; }
    }
}
