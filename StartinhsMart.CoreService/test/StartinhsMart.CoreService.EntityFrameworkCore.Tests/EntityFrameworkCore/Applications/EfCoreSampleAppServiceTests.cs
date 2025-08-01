using StartinhsMart.CoreService.Samples;
using Xunit;

namespace StartinhsMart.CoreService.EntityFrameworkCore.Applications;

[Collection(CoreServiceTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<CoreServiceEntityFrameworkCoreTestModule>
{

}
