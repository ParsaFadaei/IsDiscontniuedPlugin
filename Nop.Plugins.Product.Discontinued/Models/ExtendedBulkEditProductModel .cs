using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Product.Discontinued.Models
{
    public class ExtendedBulkEditProductModel : BulkEditProductModel
    {
        [NopResourceDisplayName("Admin.Catalog.BulkEdit.Fields.Discontinued")]
        public bool Discontinued { get; set; }
    }
}
