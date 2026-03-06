using Volo.Abp.Modularity;

namespace Nblity;

[DependsOn(
    typeof(NblityApplicationModule),
    typeof(NblityDomainTestModule)
)]
public class NblityApplicationTestModule : AbpModule
{

}
