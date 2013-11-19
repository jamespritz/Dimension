using intrinsic.xl8.model.client;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace intrinsic.dimension.model {
    public class XType: IXType {

        public int ID { get; set; }
        public int literal_resourceID { get; set; }
        public int? desc_resourceID { get; set; }

        [Timestamp]
        public byte[] rv { get; set; }

        public virtual Resource Literal { get; set; }
        public virtual Resource Description { get; set; }
        IResource IXType.Literal { get { return (IResource)this.Literal; } }
        IResource IXType.Description { get { return (IResource)this.Description; } }
    }

    public class XTypeMapping : EntityTypeConfiguration<XType> {

        internal XTypeMapping() {
            this.HasKey(k => k.ID).ToTable("XType", "dxdimension");

        }

    }
}
