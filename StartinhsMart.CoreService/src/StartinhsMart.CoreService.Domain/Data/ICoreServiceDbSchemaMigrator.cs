using System.Threading.Tasks;

namespace StartinhsMart.CoreService.Data;

public interface ICoreServiceDbSchemaMigrator
{
    Task MigrateAsync();
}
