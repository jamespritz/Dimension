using System;

namespace intrinsic.diagnostics {
    public interface ISession {

        int id { get; }
        Guid UID { get; }
        DateTimeOffset createDT { get; }
        DateTimeOffset? expireDT { get; }
        string origin { get; }

    }
}
