using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using intrinsic.security;

namespace intrinsic.diagnostics.facade {
    public interface ISessionFacade {

        ISession Create(string origin);
        void LinkAccount(ISession session, IAccount account);
        ISession SlideExpiration(ISession session, TimeSpan duration);

        void Log(LogEntry entry, ISession session);
        void Log(LogEntry[] entries, ISession session);

    }
}
