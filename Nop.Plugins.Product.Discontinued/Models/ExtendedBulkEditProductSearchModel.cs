using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Product.Discontinued.Models
{
    public class ExtendedBulkEditProductSearchModel: BulkEditProductSearchModel
    {
        public ExtendedBulkEditProductSearchModel()
        {
            AvailableStores = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Admin.Catalog.BulkEdit.List.Discontinued")]
        public bool? SearchDiscontinued { get; set; }
        [NopResourceDisplayName("Admin.Catalog.BulkEdit.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }
}
