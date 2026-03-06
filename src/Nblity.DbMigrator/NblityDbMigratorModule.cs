using Nblity.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Nblity.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(NblityEntityFrameworkCoreModule),
    typeof(NblityApplicationContractsModule)
)]
public class NblityDbMigratorModule : AbpModule
{
}
