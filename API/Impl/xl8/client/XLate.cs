
using System.Data.Entity.ModelConfiguration;
namespace intrinsic.xl8.client {

    public class XLate: IXLate {
        public int ID { get; set; }
        public int resourceID { get; set; }
        public int LCID { get; set; }

        public virtual Resource Resource { get; set; }


    }

    internal class XLateMap : EntityTypeConfiguration<XLate> {

        public XLateMap() {
            this.HasKey(k => k.ID).ToTable("xlate", "dxxlate");

        }
    }

}
