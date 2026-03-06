using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Nblity.Data;

/* This is used if database provider does't define
 * INblityDbSchemaMigrator implementation.
 */
public class NullNblityDbSchemaMigrator : INblityDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
