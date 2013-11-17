using System;
using System.Data.Entity.ModelConfiguration;

namespace intrinsic.diagnostics {
    
    public class Log: ILog {
        public int ID { get; set; }
        public Guid logTypeID { get; set; }
        public string entry { get; set; }
        public int severity { get; set; }
        public DateTimeOffset createDT { get; set; }
        public int? referenceKey { get; set; }
        public int? sessionID { get; set; }

        public virtual Session Session { get; set; }

        
    }

    internal class LogMap : EntityTypeConfiguration<Log> {
        public LogMap() {
            this.HasKey(k => k.ID).ToTable("Log", "dxdiag");
            this.HasRequired(r => r.Session).WithMany(r => r.LogEntries).HasForeignKey(k => k.sessionID);
        }
    }
}
