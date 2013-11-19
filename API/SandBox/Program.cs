using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandBox {
    class Program {
        static void Main(string[] args) {

            CreateSessionAndLog();

        }


        static void CreateSessionAndLog() {

            intrinsic.diagnostics.facade.ISessionFacade f = new intrinsic.diagnostics.facade.SessionFacade();
            intrinsic.diagnostics.model.ISession newses = f.Create("origin");


        }
    
    }



}
