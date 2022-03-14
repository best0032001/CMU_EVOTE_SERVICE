using Evote_Service.Model.Entity;
using Evote_Service.Model.Entity.Ref;
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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
        public DbSet<EventStatus> EventStatus { get; set; }
        public DbSet<ConfirmVoter> confirmVoters { get; set; }
        public DbSet<VoteEntity> voteEntities { get; set; }
        public DbSet<VoterEntity> VoterEntitys { get; set; }
        public DbSet<AdminLoginLog> AdminLoginLogs { get; set; }
        public DbSet<EventVoteEntity> EventVoteEntitys { get; set; }
        public DbSet<ApplicationEntity> ApplicationEntitys { get; set; }
        public DbSet<UserEntity> UserEntitys { get; set; }
        public DbSet<RefUserStage> RefUserStages { get; set; }
        public DbSet<UserAdminEntity> UserAdminEntitys { get; set; }
    }
}
