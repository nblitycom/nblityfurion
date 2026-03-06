using Volo.Abp.Modularity;

namespace Nblity;

public abstract class NblityApplicationTestBase<TStartupModule> : NblityTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
