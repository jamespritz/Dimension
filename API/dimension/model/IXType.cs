using intrinsic.xl8.model.client;

namespace intrinsic.dimension.model {
    public interface IXType {
        int ID { get; }

        IResource Literal { get; }
        IResource Description { get; }

    }
}
