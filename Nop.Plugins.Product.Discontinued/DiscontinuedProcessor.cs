using System;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Plugin.Product.Discontinued.Data;

namespace Nop.Plugins.Product.Discontinued
{
    public class DiscontinuedProcessor : BasePlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly DiscontinuedObjectContext _discontinuedObjectContext;

        public DiscontinuedProcessor(IWebHelper webHelper,DiscontinuedObjectContext discontinuedObjectContext)
        {
            this._webHelper = webHelper;
            this._discontinuedObjectContext = discontinuedObjectContext;
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/ProductDiscontinued/Configure";
        }

        public override void Install()
        {
            _discontinuedObjectContext.Install();
            base.Install();
        }

        public override void Uninstall()
        {
            _discontinuedObjectContext.Uninstall();
            base.Uninstall();
        }
    }
}
