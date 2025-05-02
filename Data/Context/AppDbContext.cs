using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sample_ACID.Data.Models;

namespace Sample_ACID.Data.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=AcidDemoDb;Integrated Security=True;Trust Server Certificate=True");
        }
    }
}
