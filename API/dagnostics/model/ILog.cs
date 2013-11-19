using System;

namespace intrinsic.diagnostics.model {
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
