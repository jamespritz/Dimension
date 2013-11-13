using System.Data.Entity;

namespace intrinsic.data
{
    public class DxContext: DbContext {

        static DxContext(){
            Database.SetInitializer<DxContext>(null);
           
        }

        DbSet<xl8.client.Resource> Resources { get; set; }
        DbSet<xl8.client.XLate> Translations { get; set; }

        DbSet<diagnostics.Log> LogEntries { get; set; }
        DbSet<diagnostics.Session> Sessions { get; set; }

        DbSet<security.Account> Accounts { get; set; }
        DbSet<security.Credential> Credentials { get; set; }
        
        public DxContext()
            :base("dxcontext"){

            base.Configuration.LazyLoadingEnabled = false;

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {

            modelBuilder.Configurations.Add(new xl8.client.ResourceMap());
            modelBuilder.Configurations.Add(new xl8.client.XLateMap());

            modelBuilder.Configurations.Add(new diagnostics.LogMap());
            modelBuilder.Configurations.Add(new diagnostics.SessionMap());

            modelBuilder.Configurations.Add(new security.CredentialMap());
            modelBuilder.Configurations.Add(new security.AccountMap());

            base.OnModelCreating(modelBuilder);
        }

    }
}
