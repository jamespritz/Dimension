using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using intrinsic.security;
using intrinsic.data;

namespace intrinsic.diagnostics.facade {
    public class SessionFacade: ISessionFacade {

        ISession ISessionFacade.Create(string origin) {

            DxContext ctx = new DxContext();
            IRepository<Session> repo_session = new Repository<Session>(ctx);

            Session newSession = new Session() {
                createDT = DateTimeOffset.UtcNow
                , expireDT = null
                , origin = origin
                , UID = Guid.NewGuid()
            };

            repo_session.Insert(newSession);
            ctx.SaveChanges();

            return newSession;

        }

        void ISessionFacade.LinkAccount(ISession session, IAccount account) {
            DxContext ctx = new DxContext();
            IRepository<AccountSession> repo_session = new Repository<AccountSession>(ctx);

            AccountSession joined = new AccountSession() {
                accountID = account.ID
                , sessionID = session.id
            };

            repo_session.Insert(joined);
            ctx.SaveChanges();
            

        }

        ISession ISessionFacade.SlideExpiration(ISession session, TimeSpan duration) {

            DateTimeOffset expires = (session.expireDT ?? DateTimeOffset.UtcNow).Add(duration);
            Session asSession = (Session)session;
            asSession.expireDT = expires;

            DxContext ctx = new DxContext();
            IRepository<Session> repo_session = new Repository<Session>(ctx);

            repo_session.Update(asSession);

            ctx.SaveChanges();

            return asSession;

        }

        void ISessionFacade.Log(LogEntry entry, ISession session) {
            DxContext ctx = new DxContext();
            IRepository<Log> repo_log = new Repository<Log>(ctx);

            Log newEntry = new Log() { 
                createDT = DateTime.UtcNow
                , entry = entry.ToString()
                , logTypeID = entry.LogTypeID
                , referenceKey = entry.Reference
                , sessionID = session.id
                , severity = (int)entry.Level
            };

            repo_log.Insert(newEntry);


            ctx.SaveChanges();
        }

        void ISessionFacade.Log(LogEntry[] entries, ISession session) {
            
            DxContext ctx = new DxContext();
            IRepository<Log> repo_log = new Repository<Log>(ctx);

            foreach (LogEntry l in entries) {
                
                Log newEntry = new Log() { 
                    createDT = DateTime.UtcNow
                    , entry = l.ToString()
                    , logTypeID = l.LogTypeID
                    , referenceKey = l.Reference
                    , sessionID = session.id
                    , severity = (int)l.Level
                };

                repo_log.Insert(newEntry);
            }

            ctx.SaveChanges();
            

        }
    }
}
