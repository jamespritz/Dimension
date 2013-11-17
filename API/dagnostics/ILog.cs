using System;

namespace intrinsic.diagnostics {
    public interface ILog {
        int ID { get; }
        Guid logTypeID { get; }
        string entry { get; }
        int severity { get; }
        DateTimeOffset createDT { get; }
        int? referenceKey { get; }
        int? sessionID { get; }
    }
}
