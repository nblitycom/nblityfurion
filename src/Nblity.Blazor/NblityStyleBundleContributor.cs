using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace Nblity.Blazor;

public class NblityStyleBundleContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.Add(new BundleFile("main.css", true));
    }
}
