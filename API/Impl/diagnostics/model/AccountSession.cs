using intrinsic.security.model;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace intrinsic.diagnostics.model {
    


    public class AccountSession: IAccountSession {

        public int id { get; set; }
        public int accountID { get; set; }
        public int sessionID { get; set; }

        [Timestamp]
        public byte[] rv { get; set; }

        public virtual Account Account { get; set; }    
        

    }

    internal class AccountSessionMap : EntityTypeConfiguration<AccountSession> {

        public AccountSessionMap() {

            this.HasKey(k => k.id).ToTable("accountsession", "dxsecurity");


        }
    }



}
