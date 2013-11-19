using System.Data.Entity;

namespace intrinsic.data
{
    public class DxContext: DbContext {

        static DxContext(){
            Database.SetInitializer<DxContext>(null);
           
        }

        DbSet<xl8.model.client.Resource> Resources { get; set; }
        DbSet<xl8.model.client.XLate> Translations { get; set; }

        DbSet<diagnostics.model.Log> LogEntries { get; set; }
        DbSet<diagnostics.model.Session> Sessions { get; set; }

        DbSet<security.model.Account> Accounts { get; set; }
        DbSet<security.model.Credential> Credentials { get; set; }
        
        public DxContext()
            :base("dxcontext"){

            base.Configuration.LazyLoadingEnabled = false;

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {

            modelBuilder.Configurations.Add(new xl8.model.client.ResourceMap());
            modelBuilder.Configurations.Add(new xl8.model.client.XLateMap());

            modelBuilder.Configurations.Add(new diagnostics.model.LogMap());
            modelBuilder.Configurations.Add(new diagnostics.model.SessionMap());

            modelBuilder.Configurations.Add(new security.model.CredentialMap());
            modelBuilder.Configurations.Add(new security.model.AccountMap());

            base.OnModelCreating(modelBuilder);
        }

    }
}
