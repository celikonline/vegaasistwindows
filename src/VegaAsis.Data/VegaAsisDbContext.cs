using System.Data.Entity;
using VegaAsis.Data.Entities;

namespace VegaAsis.Data
{
    public class VegaAsisDbContext : DbContext
    {
        public VegaAsisDbContext()
            : base("name=VegaAsisDbContext")
        {
        }

        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<UserSettings> UserSettings { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<GroupMember> GroupMembers { get; set; }
        public virtual DbSet<QuotaSettings> QuotaSettings { get; set; }
        public virtual DbSet<CompanySettings> CompanySettings { get; set; }
        public virtual DbSet<AppSettings> AppSettings { get; set; }
        public virtual DbSet<Offer> Offers { get; set; }
        public virtual DbSet<Policy> Policies { get; set; }
        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<WebUser> WebUsers { get; set; }
        public virtual DbSet<SharedCompany> SharedCompanies { get; set; }
        public virtual DbSet<ReceivedCompanyShare> ReceivedCompanyShares { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");
        }
    }
}
