using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Plugin.Product.Discontinued.Factories;
using Nop.Plugin.Product.Discontinued.Models;
using Nop.Plugin.Product.Discontinued.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Product.Discontinued.Controllers
{
    public class PluginProductController : Nop.Web.Areas.Admin.Controllers.ProductController
    {
        #region stuff

        private readonly IAclService _aclService;
        private readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
        private readonly ICategoryService _categoryService;
        private readonly ICopyProductService _copyProductService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IDownloadService _downloadService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IManufacturerService _manufacturerService;
        private readonly INopFileProvider _fileProvider;
        private readonly IPdfService _pdfService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly ISettingService _settingService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly VendorSettings _vendorSettings;
        private readonly IPluginProductModelFactory _pluginProductModelFactory;
        private readonly IDiscontinuedService _discontinuedService;


        #endregion

        #region constructor
        public PluginProductController(IDiscontinuedService discontinuedService, IPluginProductModelFactory pluginProductModelFactory,IAclService aclService, IBackInStockSubscriptionService backInStockSubscriptionService, ICategoryService categoryService, ICopyProductService copyProductService, ICustomerActivityService customerActivityService, ICustomerService customerService, IDiscountService discountService, IDownloadService downloadService, IExportManager exportManager, IImportManager importManager, ILanguageService languageService, ILocalizationService localizationService, ILocalizedEntityService localizedEntityService, IManufacturerService manufacturerService, INopFileProvider fileProvider, IPdfService pdfService, IPermissionService permissionService, IPictureService pictureService, IProductAttributeParser productAttributeParser, IProductAttributeService productAttributeService, IProductModelFactory productModelFactory, IProductService productService, IProductTagService productTagService, ISettingService settingService, IShippingService shippingService, IShoppingCartService shoppingCartService, ISpecificationAttributeService specificationAttributeService, IStoreMappingService storeMappingService, IStoreService storeService, IUrlRecordService urlRecordService, IWorkContext workContext, VendorSettings vendorSettings) : base(aclService, backInStockSubscriptionService, categoryService, copyProductService, customerActivityService, customerService, discountService, downloadService, exportManager, importManager, languageService, localizationService, localizedEntityService, manufacturerService, fileProvider, pdfService, permissionService, pictureService, productAttributeParser, productAttributeService, productModelFactory, productService, productTagService, settingService, shippingService, shoppingCartService, specificationAttributeService, storeMappingService, storeService, urlRecordService, workContext, vendorSettings)
        {
            this._aclService = aclService;
            this._backInStockSubscriptionService = backInStockSubscriptionService;
            this._categoryService = categoryService;
            this._copyProductService = copyProductService;
            this._customerActivityService = customerActivityService;
            this._customerService = customerService;
            this._discountService = discountService;
            this._downloadService = downloadService;
            this._exportManager = exportManager;
            this._importManager = importManager;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._manufacturerService = manufacturerService;
            this._fileProvider = fileProvider;
            this._pdfService = pdfService;
            this._permissionService = permissionService;
            this._pictureService = pictureService;
            this._productAttributeParser = productAttributeParser;
            this._productAttributeService = productAttributeService;
            this._productModelFactory = productModelFactory;
            this._productService = productService;
            this._productTagService = productTagService;
            this._settingService = settingService;
            this._shippingService = shippingService;
            this._shoppingCartService = shoppingCartService;
            this._specificationAttributeService = specificationAttributeService;
            this._storeMappingService = storeMappingService;
            this._storeService = storeService;
            this._urlRecordService = urlRecordService;
            this._workContext = workContext;
            this._vendorSettings = vendorSettings;
            this._pluginProductModelFactory = pluginProductModelFactory;
            this._discontinuedService = discontinuedService;
        }



        #endregion

        #region methods

        [Route("/Admin/Product/Create", Name = "Nop.Plugin.Product.Discontinued.Product.Create", Order = -1)]
        public override IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //validate maximum number of products per vendor
            if (_vendorSettings.MaximumProductNumber > 0 && _workContext.CurrentVendor != null
                                                         && _productService.GetNumberOfProductsByVendorId(_workContext
                                                             .CurrentVendor.Id) >= _vendorSettings.MaximumProductNumber)
            {
                ErrorNotification(string.Format(
                    _localizationService.GetResource("Admin.Catalog.Products.ExceededMaximumNumber"),
                    _vendorSettings.MaximumProductNumber));
                return RedirectToAction("List", "Product");
            }

            //prepare model
            var model = _pluginProductModelFactory.PreparePluginProductModel(new ProductDiscontinuedModel(), null);
            return View("~/Plugins/Product.Discontinued/Views/Product/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [Route("/Admin/Product/Create", Name = "Nop.Plugin.Product.Discontinued.Product.Create", Order = -1)]
        public  IActionResult Create(ProductDiscontinuedModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //validate maximum number of products per vendor
            if (_vendorSettings.MaximumProductNumber > 0 && _workContext.CurrentVendor != null
                                                         && _productService.GetNumberOfProductsByVendorId(_workContext
                                                             .CurrentVendor.Id) >= _vendorSettings.MaximumProductNumber)
            {
                ErrorNotification(string.Format(
                    _localizationService.GetResource("Admin.Catalog.Products.ExceededMaximumNumber"),
                    _vendorSettings.MaximumProductNumber));
                return RedirectToAction("List", "Product");
            }

            if (ModelState.IsValid)
            {
                //a vendor should have access only to his products
                if (_workContext.CurrentVendor != null)
                    model.VendorId = _workContext.CurrentVendor.Id;

                //vendors cannot edit "Show on home page" property
                if (_workContext.CurrentVendor != null && model.ShowOnHomePage)
                    model.ShowOnHomePage = false;

                //product
                var product = model.ToEntity<Core.Domain.Catalog.Product>();
                product.CreatedOnUtc = DateTime.UtcNow;
                product.UpdatedOnUtc = DateTime.UtcNow;
                _productService.InsertProduct(product);
                _discontinuedService.InsertAndUpdateDiscontinued(product.Id,model.DiscontinuedState);


                //search engine name
                model.SeName = _urlRecordService.ValidateSeName(product, model.SeName, product.Name, true);
                _urlRecordService.SaveSlug(product, model.SeName, 0);

                //locales
                UpdateLocales(product, model);

                //categories
                SaveCategoryMappings(product, model);

                //manufacturers
                SaveManufacturerMappings(product, model);

                //ACL (customer roles)
                SaveProductAcl(product, model);

                //stores
                SaveStoreMappings(product, model);

                //discounts
                SaveDiscountMappings(product, model);

                //tags
                _productTagService.UpdateProductTags(product, ParseProductTags(model.ProductTags));

                //warehouses
                SaveProductWarehouseInventory(product, model);

                //quantity change history
                _productService.AddStockQuantityHistoryEntry(product, product.StockQuantity, product.StockQuantity,
                    product.WarehouseId,
                    _localizationService.GetResource("Admin.StockQuantityHistory.Messages.Edit"));

                //activity log
                _customerActivityService.InsertActivity("AddNewProduct",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewProduct"), product.Name),
                    product);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Added"));

                if (!continueEditing)
                    return RedirectToAction("List", "Product");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new {id = product.Id});
            }

            //prepare model
            model = _pluginProductModelFactory.PreparePluginProductModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View("~/Plugins/Product.Discontinued/Views/Product/Create.cshtml", model);
        }

        [Route("/Admin/Product/Edit/{id}", Name = "Nop.Plugin.Product.Discontinued.Product.Edit", Order = -1)]
        public override IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = _productService.GetProductById(id);
            if (product == null || product.Deleted)
                return RedirectToAction("List", "Product");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List","Product");

            //prepare model
            var model = _pluginProductModelFactory.PreparePluginProductModel(null, product);
            model.DiscontinuedState = _discontinuedService.GetDiscontinuedById(model.Id);

            return View("~/Plugins/Product.Discontinued/Views/Product/Edit.cshtml",model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [Route("/Admin/Product/Edit/{id}", Name = "Nop.Plugin.Product.Discontinued.Product.Edit", Order = -1)]
        public IActionResult Edit(ProductDiscontinuedModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = _productService.GetProductById(model.Id);
            if (product == null || product.Deleted)
                return RedirectToAction("List", "Product");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");

            //check if the product quantity has been changed while we were editing the product
            //and if it has been changed then we show error notification
            //and redirect on the editing page without data saving
            if (product.StockQuantity != model.LastStockQuantity)
            {
                ErrorNotification(
                    _localizationService.GetResource("Admin.Catalog.Products.Fields.StockQuantity.ChangedWarning"));
                return RedirectToAction("Edit", new {id = product.Id});
            }

            if (ModelState.IsValid)
            {
                //a vendor should have access only to his products
                if (_workContext.CurrentVendor != null)
                    model.VendorId = _workContext.CurrentVendor.Id;

                //we do not validate maximum number of products per vendor when editing existing products (only during creation of new products)
                //vendors cannot edit "Show on home page" property
                if (_workContext.CurrentVendor != null && model.ShowOnHomePage != product.ShowOnHomePage)
                    model.ShowOnHomePage = product.ShowOnHomePage;

                //some previously used values
                var prevTotalStockQuantity = _productService.GetTotalStockQuantity(product);
                var prevDownloadId = product.DownloadId;
                var prevSampleDownloadId = product.SampleDownloadId;
                var previousStockQuantity = product.StockQuantity;
                var previousWarehouseId = product.WarehouseId;

                //product
                product = model.ToEntity(product);

                product.UpdatedOnUtc = DateTime.UtcNow;
                _productService.UpdateProduct(product);
                _discontinuedService.InsertAndUpdateDiscontinued(product.Id,model.DiscontinuedState);

                //search engine name
                model.SeName = _urlRecordService.ValidateSeName(product, model.SeName, product.Name, true);
                _urlRecordService.SaveSlug(product, model.SeName, 0);

                //locales
                UpdateLocales(product, model);

                //tags
                _productTagService.UpdateProductTags(product, ParseProductTags(model.ProductTags));

                //warehouses
                SaveProductWarehouseInventory(product, model);

                //categories
                SaveCategoryMappings(product, model);

                //manufacturers
                SaveManufacturerMappings(product, model);

                //ACL (customer roles)
                SaveProductAcl(product, model);

                //stores
                SaveStoreMappings(product, model);

                //discounts
                SaveDiscountMappings(product, model);

                //picture seo names
                UpdatePictureSeoNames(product);

                //back in stock notifications
                if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                    product.BackorderMode == BackorderMode.NoBackorders &&
                    product.AllowBackInStockSubscriptions &&
                    _productService.GetTotalStockQuantity(product) > 0 &&
                    prevTotalStockQuantity <= 0 &&
                    product.Published &&
                    !product.Deleted)
                {
                    _backInStockSubscriptionService.SendNotificationsToSubscribers(product);
                }

                //delete an old "download" file (if deleted or updated)
                if (prevDownloadId > 0 && prevDownloadId != product.DownloadId)
                {
                    var prevDownload = _downloadService.GetDownloadById(prevDownloadId);
                    if (prevDownload != null)
                        _downloadService.DeleteDownload(prevDownload);
                }

                //delete an old "sample download" file (if deleted or updated)
                if (prevSampleDownloadId > 0 && prevSampleDownloadId != product.SampleDownloadId)
                {
                    var prevSampleDownload = _downloadService.GetDownloadById(prevSampleDownloadId);
                    if (prevSampleDownload != null)
                        _downloadService.DeleteDownload(prevSampleDownload);
                }

                //quantity change history
                if (previousWarehouseId != product.WarehouseId)
                {
                    //warehouse is changed 
                    //compose a message
                    var oldWarehouseMessage = string.Empty;
                    if (previousWarehouseId > 0)
                    {
                        var oldWarehouse = _shippingService.GetWarehouseById(previousWarehouseId);
                        if (oldWarehouse != null)
                            oldWarehouseMessage =
                                string.Format(
                                    _localizationService.GetResource(
                                        "Admin.StockQuantityHistory.Messages.EditWarehouse.Old"), oldWarehouse.Name);
                    }

                    var newWarehouseMessage = string.Empty;
                    if (product.WarehouseId > 0)
                    {
                        var newWarehouse = _shippingService.GetWarehouseById(product.WarehouseId);
                        if (newWarehouse != null)
                            newWarehouseMessage =
                                string.Format(
                                    _localizationService.GetResource(
                                        "Admin.StockQuantityHistory.Messages.EditWarehouse.New"), newWarehouse.Name);
                    }

                    var message =
                        string.Format(
                            _localizationService.GetResource("Admin.StockQuantityHistory.Messages.EditWarehouse"),
                            oldWarehouseMessage, newWarehouseMessage);

                    //record history
                    _productService.AddStockQuantityHistoryEntry(product, -previousStockQuantity, 0,
                        previousWarehouseId, message);
                    _productService.AddStockQuantityHistoryEntry(product, product.StockQuantity, product.StockQuantity,
                        product.WarehouseId, message);
                }
                else
                {
                    _productService.AddStockQuantityHistoryEntry(product, product.StockQuantity - previousStockQuantity,
                        product.StockQuantity,
                        product.WarehouseId,
                        _localizationService.GetResource("Admin.StockQuantityHistory.Messages.Edit"));
                }

                //activity log
                _customerActivityService.InsertActivity("EditProduct",
                    string.Format(_localizationService.GetResource("ActivityLog.EditProduct"), product.Name), product);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List","Product");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new {id = product.Id});
            }

            //prepare model
            model = _pluginProductModelFactory.PreparePluginProductModel(model, product, true);

            //if we got this far, something failed, redisplay form
            return View("~/Plugins/Product.Discontinued/Views/Product/Create.cshtml", model);
        }

        #endregion

        #region cock
        [Route("/Admin/Product/BulkEdit", Name = "Nop.Plugin.Product.Discontinued.Product.BulkEdit", Order = -1)]
        public override IActionResult BulkEdit()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            
            //prepare model
            var model = _pluginProductModelFactory.PrepareExtendedBulkEditProductSearchModel(new ExtendedBulkEditProductSearchModel());

            return View("~/Plugins/Product.Discontinued/Views/Product/BulkEdit.cshtml", model);

        }

        [HttpPost]
        [Route("/Admin/Product/BulkEdit/Select", Name = "Nop.Plugin.Product.Discontinued.Product.BulkEditSelect", Order = -1)]
        public IActionResult BulkEditSelect(ExtendedBulkEditProductSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _pluginProductModelFactory.PrepareExtendedBulkEditProductListModel(searchModel);
            
            return Json(model);
        }

        [HttpPost]
        [Route("/Admin/Product/BulkEdit/Update", Name = "Nop.Plugin.Product.Discontinued.Product.BulkEditUpdate", Order = -1)]
        public IActionResult BulkEditUpdate(IEnumerable<ExtendedBulkEditProductModel> products)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (products == null)
                return new NullJsonResult();

            foreach (var pModel in products)
            {
                //update
                var product = _productService.GetProductById(pModel.Id);
                if (product == null)
                    continue;

                //a vendor should have access only to his products
                if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                    continue;

                var prevTotalStockQuantity = _productService.GetTotalStockQuantity(product);
                var previousStockQuantity = product.StockQuantity;
                product.Name = pModel.Name;
                product.Sku = pModel.Sku;
                product.Price = pModel.Price;
                product.OldPrice = pModel.OldPrice;
                product.StockQuantity = pModel.StockQuantity;
                product.Published = pModel.Published;
                product.UpdatedOnUtc = DateTime.UtcNow;
                _discontinuedService.InsertAndUpdateDiscontinued(pModel.Id, pModel.Discontinued);
                _productService.UpdateProduct(product);

                //back in stock notifications
                if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                    product.BackorderMode == BackorderMode.NoBackorders &&
                    product.AllowBackInStockSubscriptions &&
                    _productService.GetTotalStockQuantity(product) > 0 &&
                    prevTotalStockQuantity <= 0 &&
                    product.Published &&
                    !product.Deleted)
                {
                    _backInStockSubscriptionService.SendNotificationsToSubscribers(product);
                }

                //quantity change history
                _productService.AddStockQuantityHistoryEntry(product, product.StockQuantity - previousStockQuantity, product.StockQuantity,
                    product.WarehouseId, _localizationService.GetResource("Admin.StockQuantityHistory.Messages.Edit"));
            }

            return new NullJsonResult();
        }

        [HttpPost]
        [Route("/Admin/Product/BulkEdit/Delete", Name = "Nop.Plugin.Product.Discontinued.Product.BulkEditDelete", Order = -1)]
        public virtual IActionResult BulkEditDelete(IEnumerable<ExtendedBulkEditProductModel> products)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (products == null)
                return new NullJsonResult();

            foreach (var pModel in products)
            {
                //delete
                var product = _productService.GetProductById(pModel.Id);
                if (product == null)
                    continue;

                //a vendor should have access only to his products
                if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                    continue;

                _productService.DeleteProduct(product);
            }

            return new NullJsonResult();
        }

        #endregion


    }


}