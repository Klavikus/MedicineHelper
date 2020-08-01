using Medicine.db;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;

namespace Medicine
{
    public class MyDbContext : DbContext
    {
        static string connect = $"data source=(localdb)\\MSSQLLocalDB;" +
            $"Initial Catalog = 123; Integrated Security = true;";
        public MyDbContext() : base(connect)
        {

        }

        public DbSet<Shedule> Shedule { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<MessageLog> Messages { get; set; }
    }
}
