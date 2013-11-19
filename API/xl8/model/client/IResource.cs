using System;
using System.Collections.Generic;

namespace intrinsic.xl8.model.client {

    /// <summary>
    /// uniquely identifies a literal resources that has one to many translations
    /// </summary>
    public interface IResource {
        int ID { get; }
        Guid UID { get; }
        List<IXLate> Translations { get; }
    }
}
