using Volo.Abp.Modularity;

namespace Nblity;

/* Inherit from this class for your domain layer tests. */
public abstract class NblityDomainTestBase<TStartupModule> : NblityTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
