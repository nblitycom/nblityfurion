using Volo.Abp.Modularity;

namespace Nblity;

[DependsOn(
    typeof(NblityDomainModule),
    typeof(NblityTestBaseModule)
)]
public class NblityDomainTestModule : AbpModule
{

}
