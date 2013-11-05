using System.Data.Entity;

namespace intrinsic.data
{
    public class DxContext: DbContext {

        static DxContext(){
            Database.SetInitializer<DxContext>(null);
           
        }

        DbSet<xl8.client.Resource> Resources { get; set; }
        DbSet<xl8.client.XLate> Translations { get; set; }

        public DxContext()
            :base("dxcontext"){

            base.Configuration.LazyLoadingEnabled = false;

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {

            modelBuilder.Configurations.Add(new xl8.client.ResourceMap());
            modelBuilder.Configurations.Add(new xl8.client.XLateMap());



            base.OnModelCreating(modelBuilder);
        }

    }
}
