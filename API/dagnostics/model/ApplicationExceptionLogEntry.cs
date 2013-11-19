using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace intrinsic.diagnostics.model {

    public class TraceLogEntry: LogEntry {

        

        public TraceLogEntry(string message, TraceEventType level)
            : base(level, Guid.Parse("A0E59F28-FD53-4E08-A090-186CBB125486")) {
            base._content = message;
        }

        public TraceLogEntry(string format, TraceEventType level, params string[] args)
            : base(level, Guid.Parse("A0E59F28-FD53-4E08-A090-186CBB125486")) {

                base._content = string.Format(format, args);

        }

    }

    public class ApplicationExceptionLogEntry : LogEntry {

        
        public ApplicationExceptionLogEntry(System.Exception ex)
            : base(TraceEventType.Error, Guid.Parse("E289D41E-74FA-4E29-BD94-99EDEDA42F9D")) {

            StringBuilder sb = new StringBuilder();
            while (ex != null) {
                sb.AppendLine(string.Empty);
                sb.AppendLine(string.Format("Source:{0} {1} - {2}", ex.Source, ex.GetType(), ex.Message));
                sb.AppendLine(string.Format("Stack:{0}", ex.StackTrace));
                ex = ex.InnerException;
            }

            base._content = sb.ToString();

            


        }
    }

}
