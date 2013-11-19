using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;
using intrinsic.diagnostics.model;
using intrinsic.diagnostics.facade;
using intrinsic.data;
using System.Data.Common;
using System.Linq.Expressions;

namespace test.diagnostics {

    public class DiagnosticsServiceTest {


        private SessionFacade DumbAss {
            get {
                SessionFacade f = new SessionFacade();

                //ensures nothing gets wired to db.
                f.AccountSessionRepo = new Mock<IRepository<AccountSession>>().Object;
                f.SessionRepo = new Mock<IRepository<Session>>().Object;
                f.LogRepo = new Mock<IRepository<Log>>().Object;
                f.Dimension = new Mock<DxContext>().Object;
                return f;
            }
        }

        /// <summary>
        /// create method must throw exception if passed and
        /// empty or null origin.
        /// </summary>
        [Fact]
        public void CreateEmptyOriginEx() {

            Assert.Throws<System.ArgumentNullException>(() => {

                ISessionFacade f = this.DumbAss;
                f.Create(null);
                
            });

        }

        [Fact]
        public void LinkAccountNullAccountEx() {


            Assert.Throws<System.ArgumentNullException>(() => {

                ISessionFacade f = this.DumbAss;
                f.LinkAccount(new Mock<ISession>().Object, null);

            });            
        }

        [Fact]
        public void LinkAccountNullSessionEx() {

            Assert.Throws<System.ArgumentNullException>(() => {

                ISessionFacade f = this.DumbAss;
                f.LinkAccount(null, new Mock<intrinsic.security.model.IAccount>().Object);

            });
            
        }

        [Fact]
        public void LinkAccountAccountAlreadyLinkedEx() {


            Assert.Throws<System.InvalidOperationException>(() => {

                ISessionFacade f = this.DumbAss;

                //we need to override the db behavior to mock a resultset
                AccountSession mAcctSess = new AccountSession() {
                    accountID = 1
                    , sessionID = 1
                    , id = 1
                };

                Mock<IRepository<AccountSession>> mAccountSessionRepo = new Mock<IRepository<AccountSession>>();
                mAccountSessionRepo.Setup(m => m.Get(It.IsAny<Expression<Func<AccountSession, bool>>>(), null, It.IsAny<string>()))
                    .Returns(new List<AccountSession>() { mAcctSess });
                ((SessionFacade)f).AccountSessionRepo = mAccountSessionRepo.Object;

                f.LinkAccount(new Mock<ISession>().Object, new Mock<intrinsic.security.model.IAccount>().Object);

            });
            
        }

        [Fact]
        public void SlideExpirationAlreadyExpiredEx() {

            Assert.Throws<System.InvalidOperationException>(() => {

                ISessionFacade f = this.DumbAss;

                Session mSession = new Session { 
                    createDT = DateTimeOffset.UtcNow
                    , expireDT = DateTimeOffset.UtcNow.AddMinutes(-5)
                    , id = 1
                    , origin = "here"
                };

                f.SlideExpiration(mSession, TimeSpan.FromMinutes(30));

            });
            
        }

        [Fact]
        public void SlideExpirationPastDurationEx() {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => {

                ISessionFacade f = this.DumbAss;

                Session mSession = new Session { 
                    createDT = DateTimeOffset.UtcNow
                    , expireDT = DateTimeOffset.UtcNow.AddMinutes(30)
                    , id = 1
                    , origin = "here"
                };

                f.SlideExpiration(mSession, TimeSpan.FromMinutes(-30));

            });
            
        }

        [Fact]
        public void LogEntryNullSessionEx() {
            
            Assert.Throws<System.ArgumentNullException>(() => {

                ISessionFacade f = this.DumbAss;

                TraceLogEntry e = new TraceLogEntry("test message", System.Diagnostics.TraceEventType.Information);

                f.Log(e, null);
            });
            
        }

        [Fact]
        public void LogEntryNullEntryEx() {
            Assert.Throws<System.ArgumentNullException>(() => {

                ISessionFacade f = this.DumbAss;

                TraceLogEntry e = null;
                //new TraceLogEntry("test message", System.Diagnostics.TraceEventType.Information);

                f.Log(e, new Mock<ISession>().Object);
            });
            
            
        }

        [Fact]
        public void LinkAccountUpdatesDB() {

            ISessionFacade f = this.DumbAss;
            bool inserted = false;
            bool committed = false;

            Mock<IRepository<AccountSession>> mAcctSessRepo = new Mock<IRepository<AccountSession>>();
            mAcctSessRepo.Setup(g => g.Insert(It.IsAny<AccountSession>())).Callback(() => { inserted = true; });
            mAcctSessRepo.Setup(m => m.Get(It.IsAny<Expression<Func<AccountSession, bool>>>(), null, It.IsAny<string>()))
                .Returns(new List<AccountSession>());

            Mock<DxContext> mDimension = new Mock<DxContext>();
            mDimension.Setup(g => g.SaveChanges()).Callback(() => { committed = true; });

            ((SessionFacade)f).Dimension = mDimension.Object;
            ((SessionFacade) f).AccountSessionRepo = mAcctSessRepo.Object;

            f.LinkAccount(new Session() { id = 1 }, new intrinsic.security.model.Account() { ID = 1 });

            Assert.True(inserted && committed);
            

        }

        [Fact]
        public void LogEntryUpdatesDB() {
            ISessionFacade f = this.DumbAss;
            bool inserted = false;
            bool committed = false;

            Mock<IRepository<Log>> mLogRepo = new Mock<IRepository<Log>>();
            mLogRepo.Setup(g => g.Insert(It.IsAny<Log>())).Callback(() => { inserted = true; });
           

            Mock<DxContext> mDimension = new Mock<DxContext>();
            mDimension.Setup(g => g.SaveChanges()).Callback(() => { committed = true; });

            ((SessionFacade)f).Dimension = mDimension.Object;
            ((SessionFacade)f).LogRepo = mLogRepo.Object;

            f.Log(new TraceLogEntry("test", System.Diagnostics.TraceEventType.Information), new Session() { id = 1 });


            Assert.True(inserted && committed);
            
        }

        [Fact]
        public void SlideExpirationUpdatesDB() {
            ISessionFacade f = this.DumbAss;
            bool updated = false;
            bool committed = false;

            Mock<IRepository<Session>> mSessionRepo = new Mock<IRepository<Session>>();
            mSessionRepo.Setup(g => g.Update(It.IsAny<Session>())).Callback(() => { updated = true; });


            Mock<DxContext> mDimension = new Mock<DxContext>();
            mDimension.Setup(g => g.SaveChanges()).Callback(() => { committed = true; });

            ((SessionFacade)f).Dimension = mDimension.Object;
            ((SessionFacade)f).SessionRepo = mSessionRepo.Object;

            f.SlideExpiration(new Session() { id = 1, expireDT = DateTimeOffset.UtcNow.AddMinutes(30) }
                , TimeSpan.FromMinutes(30));


            Assert.True(updated && committed);
            
        }



    }
}
