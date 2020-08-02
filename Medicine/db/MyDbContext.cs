using Medicine.db;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;

namespace Medicine
{
    public class MyDbContext : DbContext
    {
        public static readonly IConfiguration _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        public MyDbContext() : base(_configuration["msSql"])
        {

        }

        public DbSet<Shedule> Shedule { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<MessageLog> Messages { get; set; }
    }
}
