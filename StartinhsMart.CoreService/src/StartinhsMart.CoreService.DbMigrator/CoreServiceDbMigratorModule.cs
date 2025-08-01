using StartinhsMart.CoreService.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace StartinhsMart.CoreService.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(CoreServiceEntityFrameworkCoreModule),
    typeof(CoreServiceApplicationContractsModule)
)]
public class CoreServiceDbMigratorModule : AbpModule
{
}
