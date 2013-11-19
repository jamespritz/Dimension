using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace intrinsic.diagnostics.model {


    public class Session: ISession {

        public int id { get; set; }
        public Guid UID { get; set; }
        public DateTimeOffset createDT { get; set; }
        public DateTimeOffset? expireDT { get; set; }
        public string origin { get; set; }

        public virtual List<Log> LogEntries { get; set; }
    }

    internal class SessionMap : EntityTypeConfiguration<Session> {

        public SessionMap() {
            this.HasKey(k => k.id).ToTable("Session", "dxdiag");
            
        }
    }
}
