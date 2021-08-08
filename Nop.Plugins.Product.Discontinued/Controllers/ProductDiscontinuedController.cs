using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Product.Discontinued.Models;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using SixLabors.ImageSharp;

namespace Nop.Plugin.Product.Discontinued.Controllers
{
    public class ProductDiscontinuedController:BasePluginController
    {
        private readonly IPermissionService _permissionService;
        public ProductDiscontinuedController(IPermissionService permissionService)
        {
            this._permissionService = permissionService;
        }
        [AuthorizeAdmin] 
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var model = new ConfigurationModel()
            {
                yes = true
            };
            return View("~/Plugins/Product.Discontinued/Views/Configure.cshtml", model);
        }
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult test()
        {
            return View("~/Plugins/Product.Discontinued/Views/test.cshtml");
        }
        
    }
}
