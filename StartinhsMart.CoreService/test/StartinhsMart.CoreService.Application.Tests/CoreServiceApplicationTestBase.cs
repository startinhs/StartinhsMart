using Volo.Abp.Modularity;

namespace StartinhsMart.CoreService;

public abstract class CoreServiceApplicationTestBase<TStartupModule> : CoreServiceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
