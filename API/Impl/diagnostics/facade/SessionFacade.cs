using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using intrinsic.security;
using intrinsic.data;
using Microsoft.Practices.Unity;
using System.Data.Entity;

namespace intrinsic.diagnostics.facade {

    public class SessionFacade: ISessionFacade {


        private DbContext dxContext;
        //[Dependency("dimension")]
        public DbContext Dimension {
            get {
                if (this.dxContext == null) {
                    this.dxContext = new DxContext();
                }
                return this.dxContext;
            }
            set {
                this.dxContext = value;
            }
        }

        private IRepository<Session> repo_session;
        public IRepository<Session> SessionRepo {
            get {
                if (this.repo_session == null) {
                    this.repo_session = new Repository<Session>(this.Dimension); 
                }
                return this.repo_session;
            }
            set {
                this.repo_session = value;
            }
        }

        private IRepository<AccountSession> repo_acctsession;
        public IRepository<AccountSession> AccountSessionRepo {
            get {
                if (this.repo_acctsession == null) {
                    this.repo_acctsession = new Repository<AccountSession>(this.Dimension);
                }
                return this.repo_acctsession;
            }
            set {
                this.repo_acctsession = value;
            }
        }

        private IRepository<Log> repo_log;
        public IRepository<Log> LogRepo {
            get {
                if (this.repo_log == null) {
                    this.repo_log = new Repository<Log>(this.Dimension);
                }
                return this.repo_log;
            }
            set {
                this.repo_log = value;
            }
        }

        ISession ISessionFacade.Create(string origin) {

            if (string.IsNullOrEmpty(origin)) throw new ArgumentNullException("origin");

            Session newSession = new Session() {
                createDT = DateTimeOffset.UtcNow
                , expireDT = null
                , origin = origin
                , UID = Guid.NewGuid()
            };

            SessionRepo.Insert(newSession);
            Dimension.SaveChanges();

            return newSession;

        }

        void ISessionFacade.LinkAccount(ISession session, IAccount account) {

            if (session == null) { throw new ArgumentNullException("session"); }
            if (account == null) { throw new ArgumentNullException("account"); }

   

            AccountSession current = AccountSessionRepo.Get(g => g.sessionID == session.id, null, null).FirstOrDefault();
            if (current != null) throw new InvalidOperationException("Session is already associated with an account");

            AccountSession joined = new AccountSession() {
                accountID = account.ID
                , sessionID = session.id
            };

            this.AccountSessionRepo.Insert(joined);
            this.Dimension.SaveChanges();
        }

        ISession ISessionFacade.SlideExpiration(ISession session, TimeSpan duration) {

            if (session == null) throw new ArgumentNullException("session");
            

            if (session.expireDT <= DateTimeOffset.UtcNow) throw new InvalidOperationException("Session has already been expired");
            if (duration <= TimeSpan.FromSeconds(0)) { throw new ArgumentOutOfRangeException("Duration must be greater than 0"); }

            DateTimeOffset expires = (session.expireDT ?? DateTimeOffset.UtcNow).Add(duration);
            Session asSession = (Session)session;
            asSession.expireDT = expires;

            this.SessionRepo.Update(asSession);

            this.Dimension.SaveChanges();

            return asSession;

        }

        void ISessionFacade.Log(LogEntry entry, ISession session) {

            if (entry == null) { throw new ArgumentNullException("entry"); }
            if (session == null) { throw new ArgumentNullException("session"); }
            if (session.id == 0) { throw new ArgumentException("session must be created first"); }
            

            Log newEntry = new Log() { 
                createDT = DateTime.UtcNow
                , entry = entry.ToString()
                , logTypeID = entry.LogTypeID
                , referenceKey = entry.Reference
                , sessionID = session.id
                , severity = (int)entry.Level
            };

            this.LogRepo.Insert(newEntry);


            this.Dimension.SaveChanges();
        }

        void ISessionFacade.Log(LogEntry[] entries, ISession session) {

            if ((entries == null) || (entries.Count() == 0)) { throw new ArgumentNullException("entry"); }
            if (session == null) { throw new ArgumentNullException("session"); }
            if (session.id == 0) { throw new ArgumentException("session must be created first"); }
            

            foreach (LogEntry l in entries) {
                
                Log newEntry = new Log() { 
                    createDT = DateTime.UtcNow
                    , entry = l.ToString()
                    , logTypeID = l.LogTypeID
                    , referenceKey = l.Reference
                    , sessionID = session.id
                    , severity = (int)l.Level
                };

                this.LogRepo.Insert(newEntry);
            }

            this.Dimension.SaveChanges();
            

        }
    }
}
