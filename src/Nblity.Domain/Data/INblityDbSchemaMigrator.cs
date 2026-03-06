using System.Threading.Tasks;

namespace Nblity.Data;

public interface INblityDbSchemaMigrator
{
    Task MigrateAsync();
}
