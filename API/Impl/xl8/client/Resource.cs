using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;

namespace intrinsic.xl8.client {
    
    
    public class Resource: IResource {
        
        public int ID { get; set; }
        public Guid UID { get; set; }

        public virtual List<XLate> Translations { get; set; }
        List<IXLate> IResource.Translations {
            get {
                return new List<IXLate>(from x in this.Translations select (IXLate)x);
            }
        }

    }

    internal class ResourceMap : EntityTypeConfiguration<Resource> {
        public ResourceMap() {
            this.HasKey(k => k.ID).ToTable("resource", "dxxl8");
            this.HasMany(r => r.Translations).WithRequired(r => r.Resource).HasForeignKey(k => k.resourceID);
        }
    }
}
