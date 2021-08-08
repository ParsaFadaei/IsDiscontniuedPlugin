using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.IdentityModel.Tokens.Saml;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Product.Discontinued.Models
{
    public class ProductCustomerDiscontinuedModel:ProductDetailsModel
    {
        public bool DiscontinuedState { get; set; }
    }
}
