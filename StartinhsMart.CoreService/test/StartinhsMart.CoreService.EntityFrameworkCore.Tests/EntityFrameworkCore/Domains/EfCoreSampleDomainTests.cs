using StartinhsMart.CoreService.Samples;
using Xunit;

namespace StartinhsMart.CoreService.EntityFrameworkCore.Domains;

[Collection(CoreServiceTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<CoreServiceEntityFrameworkCoreTestModule>
{

}
