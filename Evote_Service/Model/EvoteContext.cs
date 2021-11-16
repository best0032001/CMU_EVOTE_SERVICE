using Evote_Service.Model.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model
{
    public class EvoteContext : DbContext
    {
        public EvoteContext(DbContextOptions<EvoteContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){}

        public DbSet<UserEntity> UserEntitys { get; set; }
        public DbSet<UserAdminEntity> UserAdminEntitys { get; set; }
    }
}
