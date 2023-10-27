using Frame.Runtime.DI.Collections;
using Frame.Runtime.DI.Provider;

namespace Frame.Runtime.DI.Container
{
    public interface IDependenciesContainer
    {
        public IDependencyProvider Make();
    }
}