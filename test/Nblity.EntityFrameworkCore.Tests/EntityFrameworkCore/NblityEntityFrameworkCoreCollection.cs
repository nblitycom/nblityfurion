using Xunit;

namespace Nblity.EntityFrameworkCore;

[CollectionDefinition(NblityTestConsts.CollectionDefinitionName)]
public class NblityEntityFrameworkCoreCollection : ICollectionFixture<NblityEntityFrameworkCoreFixture>
{

}
