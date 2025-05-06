using FarmsApi.Migrations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace FarmsApi.DataModels
{
    public class Context : DbContext
    {




        public DbSet<Banks> Banks { get; set; }
        public DbSet<BanksBrunchs> BanksBrunchs { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Workers> Workers { get; set; }
        public DbSet<Cities> Cities { get; set; }
        public DbSet<Testpdfs> Testpdfs { get; set; }

        public DbSet<EsekConfiguraions> EsekConfiguraions { get; set; }
        public DbSet<Files> Files { get; set; }
        public DbSet<WorkerChilds> WorkerChilds { get; set; }

        public DbSet<Logs> Logs { get; set; }
        public DbSet<Farm> Farms { get; set; }
        public DbSet<FarmManagers> FarmManagers { get; set; }
        public DbSet<FarmInstructors> FarmInstructors { get; set; }

        public DbSet<FarmPDFFiles> FarmPDFFiles { get; set; }


        public DbSet<FieldsDataTypes> FieldsDataTypes { get; set; }
        public DbSet<FieldsGroups> FieldsGroups { get; set; }
        public DbSet<Fields> Fields { get; set; }
        public DbSet<Fields2Groups> Fields2Groups { get; set; }
        public DbSet<FieldsDDL> FieldsDDL { get; set; }







        public Context() : base("Farms") {

            // this.Configuration.ProxyCreationEnabled = false;
          
           // Database.SetInitializer(new MigrateDatabaseToLatestVersion<Context, FarmsApi.Migrations.Configuration>());
          
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Workers>().HasMany(s => s).WithOne(s => s.);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}