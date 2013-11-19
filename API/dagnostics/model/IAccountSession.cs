using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace intrinsic.diagnostics.model {

    public interface IAccountSession {

        int id { get; }
        int accountID { get; }
        int sessionID { get; }

    }
}
