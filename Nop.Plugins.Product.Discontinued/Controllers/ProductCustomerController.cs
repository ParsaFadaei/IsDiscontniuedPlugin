using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Plugin.Product.Discontinued.Factories.Customer;
using Nop.Plugin.Product.Discontinued.Services;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using IProductModelFactory = Nop.Web.Factories.IProductModelFactory;

namespace Nop.Plugin.Product.Discontinued.Controllers
{
    public class ProductCustomerController:Nop.Web.Controllers.ProductController
    {
        #region Fields

        private readonly CaptchaSettings _captchaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly ICompareProductsService _compareProductsService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductCustomerModelFactory _productCustomerModelFactory;
        private readonly IProductService _productService;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion
        public ProductCustomerController(IProductCustomerModelFactory productCustomerModelFactory, CaptchaSettings captchaSettings, CatalogSettings catalogSettings, IAclService aclService, ICompareProductsService compareProductsService, ICustomerActivityService customerActivityService, IEventPublisher eventPublisher, ILocalizationService localizationService, IOrderService orderService, IPermissionService permissionService, IProductModelFactory productModelFactory, IProductService productService, IRecentlyViewedProductsService recentlyViewedProductsService, IStoreContext storeContext, IStoreMappingService storeMappingService, IUrlRecordService urlRecordService, IWebHelper webHelper, IWorkContext workContext, IWorkflowMessageService workflowMessageService, LocalizationSettings localizationSettings, ShoppingCartSettings shoppingCartSettings) : base(captchaSettings, catalogSettings, aclService, compareProductsService, customerActivityService, eventPublisher, localizationService, orderService, permissionService, productModelFactory, productService, recentlyViewedProductsService, storeContext, storeMappingService, urlRecordService, webHelper, workContext, workflowMessageService, localizationSettings, shoppingCartSettings)
        {
            this._productCustomerModelFactory = productCustomerModelFactory;
            this._captchaSettings = captchaSettings;
            this._catalogSettings = catalogSettings;
            this._aclService = aclService;
            this._compareProductsService = compareProductsService;
            this._customerActivityService = customerActivityService;
            this._eventPublisher = eventPublisher;
            this._localizationService = localizationService;
            this._orderService = orderService;
            this._permissionService = permissionService;
            this._productModelFactory = productModelFactory;
            this._productService = productService;
            this._recentlyViewedProductsService = recentlyViewedProductsService;
            this._storeContext = storeContext;
            this._storeMappingService = storeMappingService;
            this._urlRecordService = urlRecordService;
            this._webHelper = webHelper;
            this._workContext = workContext;
            this._workflowMessageService = workflowMessageService;
            this._localizationSettings = localizationSettings;
            this._shoppingCartSettings = shoppingCartSettings;
        }
        #region Product details page

        [HttpsRequirement(SslRequirement.No)]
        public override IActionResult ProductDetails(int productId, int updatecartitemid = 0)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
                return InvokeHttp404();

            var notAvailable =
                //published?
                (!product.Published && !_catalogSettings.AllowViewUnpublishedProductPage) ||
                //ACL (access control list) 
                !_aclService.Authorize(product) ||
                //Store mapping
                !_storeMappingService.Authorize(product) ||
                //availability dates
                !_productService.ProductIsAvailable(product);
            //Check whether the current user has a "Manage products" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            if (notAvailable && !_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return InvokeHttp404();

            //visible individually?
            if (!product.VisibleIndividually)
            {
                //is this one an associated products?
                var parentGroupedProduct = _productService.GetProductById(product.ParentGroupedProductId);
                if (parentGroupedProduct == null)
                    return RedirectToRoute("HomePage");

                return RedirectToRoute("Product", new { SeName = _urlRecordService.GetSeName(parentGroupedProduct) });
            }

            //update existing shopping cart or wishlist  item?
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found?
                if (updatecartitem == null)
                {
                    return RedirectToRoute("Product", new { SeName = _urlRecordService.GetSeName(product) });
                }
                //is it this product?
                if (product.Id != updatecartitem.ProductId)
                {
                    return RedirectToRoute("Product", new { SeName = _urlRecordService.GetSeName(product) });
                }
            }

            //save as recently viewed
            _recentlyViewedProductsService.AddProductToRecentlyViewedList(product.Id);

            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) &&
                _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
            {
                //a vendor should have access only to his products
                if (_workContext.CurrentVendor == null || _workContext.CurrentVendor.Id == product.VendorId)
                {
                    DisplayEditLink(Url.Action("Edit", "Product", new { id = product.Id, area = Nop.Web.Framework.AreaNames.Admin }));
                }
            }

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewProduct",
                string.Format(_localizationService.GetResource("ActivityLog.PublicStore.ViewProduct"), product.Name), product);

            //model
            var model = _productCustomerModelFactory.PrepareDiscontinuedProductDetailsModel(product, updatecartitem, false);
            
            //template
            var productTemplateViewPath = _productModelFactory.PrepareProductTemplateViewPath(product);

            return View("~/Plugins/Product.Discontinued/Views/Customer/Product/ProductTemplate.Simple.cshtml", model);
        }
        #endregion
    }
}
