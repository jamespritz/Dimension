using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using intrinsic.security;
using System.Data.Entity.ModelConfiguration;

namespace intrinsic.diagnostics {
    internal class AccountSession {

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
