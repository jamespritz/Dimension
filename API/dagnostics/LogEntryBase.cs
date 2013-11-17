using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace intrinsic.diagnostics {
    public abstract class LogEntry {

        protected System.Diagnostics.TraceEventType _level;
        protected Guid _logTypeID = Guid.Empty;
        protected string _content = null;
        protected int? _reference = null;

        protected LogEntry(System.Diagnostics.TraceEventType level, Guid logTypeID) {
            this._level = level;
            this._logTypeID = logTypeID;
        }

        public System.Diagnostics.TraceEventType Level { get { return this._level; } }
        public Guid LogTypeID { get { return this._logTypeID; } }
        public string Content { get { return this._content; } }
        public int? Reference { get { return this._reference; } }
    }
}
