using Volo.Abp.Modularity;

namespace StartinhsMart.CoreService;

[DependsOn(
    typeof(CoreServiceDomainModule),
    typeof(CoreServiceTestBaseModule)
)]
public class CoreServiceDomainTestModule : AbpModule
{

}
