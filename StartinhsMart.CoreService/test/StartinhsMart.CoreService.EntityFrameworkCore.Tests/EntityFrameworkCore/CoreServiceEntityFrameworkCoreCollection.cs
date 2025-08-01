using Xunit;

namespace StartinhsMart.CoreService.EntityFrameworkCore;

[CollectionDefinition(CoreServiceTestConsts.CollectionDefinitionName)]
public class CoreServiceEntityFrameworkCoreCollection : ICollectionFixture<CoreServiceEntityFrameworkCoreFixture>
{

}
