using Volo.Abp.Modularity;

namespace StartinhsMart.CoreService;

[DependsOn(
    typeof(CoreServiceApplicationModule),
    typeof(CoreServiceDomainTestModule)
)]
public class CoreServiceApplicationTestModule : AbpModule
{

}
