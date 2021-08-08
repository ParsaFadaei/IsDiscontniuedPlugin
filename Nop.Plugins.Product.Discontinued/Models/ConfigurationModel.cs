using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Product.Discontinued.Models
{
    public class ConfigurationModel: BaseNopModel
    {
        public bool yes { get; set; }
    }
}
