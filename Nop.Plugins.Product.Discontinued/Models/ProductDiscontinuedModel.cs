using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.IdentityModel.Tokens.Saml;

namespace Nop.Plugin.Product.Discontinued.Models
{
    public class ProductDiscontinuedModel:Nop.Web.Areas.Admin.Models.Catalog.ProductModel
    {
        public bool DiscontinuedState { get; set; }
    }
}
