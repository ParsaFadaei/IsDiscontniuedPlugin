using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.Mapper;
using Nop.Core.Plugins;
using Nop.Plugin.Product.Discontinued.Models;
using Nop.Services.Seo;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Product.Discontinued.Mapper
{

    public class MapperConfiguration:Profile,IOrderedMapperProfile
    {
        public MapperConfiguration()
        {
            //create specific maps
            CreateCatalogMaps();
            
                //add some generic mapping rules
                ForAllMaps((mapConfiguration, map) =>
                {
                    //exclude Form and CustomProperties from mapping BaseNopModel
                    if (typeof(BaseNopModel).IsAssignableFrom(mapConfiguration.DestinationType))
                    {
                        map.ForMember(nameof(BaseNopModel.Form), options => options.Ignore());
                        map.ForMember(nameof(BaseNopModel.CustomProperties), options => options.Ignore());
                    }

                    //exclude ActiveStoreScopeConfiguration from mapping ISettingsModel
                    if (typeof(ISettingsModel).IsAssignableFrom(mapConfiguration.DestinationType))
                        map.ForMember(nameof(ISettingsModel.ActiveStoreScopeConfiguration), options => options.Ignore());

                    //exclude Locales from mapping ILocalizedModel
                    if (typeof(ILocalizedModel).IsAssignableFrom(mapConfiguration.DestinationType))
                        map.ForMember(nameof(ILocalizedModel<ILocalizedModel>.Locales), options => options.Ignore());

                    //exclude some properties from mapping store mapping supported entities and models
                    if (typeof(IStoreMappingSupported).IsAssignableFrom(mapConfiguration.DestinationType))
                        map.ForMember(nameof(IStoreMappingSupported.LimitedToStores), options => options.Ignore());
                    if (typeof(IStoreMappingSupportedModel).IsAssignableFrom(mapConfiguration.DestinationType))
                    {
                        map.ForMember(nameof(IStoreMappingSupportedModel.AvailableStores), options => options.Ignore());
                        map.ForMember(nameof(IStoreMappingSupportedModel.SelectedStoreIds), options => options.Ignore());
                    }

                    //exclude some properties from mapping ACL supported entities and models
                    if (typeof(IAclSupported).IsAssignableFrom(mapConfiguration.DestinationType))
                        map.ForMember(nameof(IAclSupported.SubjectToAcl), options => options.Ignore());
                    if (typeof(IAclSupportedModel).IsAssignableFrom(mapConfiguration.DestinationType))
                    {
                        map.ForMember(nameof(IAclSupportedModel.AvailableCustomerRoles), options => options.Ignore());
                        map.ForMember(nameof(IAclSupportedModel.SelectedCustomerRoleIds), options => options.Ignore());
                    }

                    //exclude some properties from mapping discount supported entities and models
                    if (typeof(IDiscountSupported).IsAssignableFrom(mapConfiguration.DestinationType))
                        map.ForMember(nameof(IDiscountSupported.AppliedDiscounts), options => options.Ignore());
                    if (typeof(IDiscountSupportedModel).IsAssignableFrom(mapConfiguration.DestinationType))
                    {
                        map.ForMember(nameof(IDiscountSupportedModel.AvailableDiscounts), options => options.Ignore());
                        map.ForMember(nameof(IDiscountSupportedModel.SelectedDiscountIds), options => options.Ignore());
                    }

                    if (typeof(IPluginModel).IsAssignableFrom(mapConfiguration.DestinationType))
                    {
                        //exclude some properties from mapping plugin models
                        map.ForMember(nameof(IPluginModel.ConfigurationUrl), options => options.Ignore());
                        map.ForMember(nameof(IPluginModel.IsActive), options => options.Ignore());
                        map.ForMember(nameof(IPluginModel.LogoUrl), options => options.Ignore());

                        //define specific rules for mapping plugin models
                        if (typeof(IPlugin).IsAssignableFrom(mapConfiguration.SourceType))
                        {
                            map.ForMember(nameof(IPluginModel.DisplayOrder), options => options.MapFrom(plugin => ((IPlugin)plugin).PluginDescriptor.DisplayOrder));
                            map.ForMember(nameof(IPluginModel.FriendlyName), options => options.MapFrom(plugin => ((IPlugin)plugin).PluginDescriptor.FriendlyName));
                            map.ForMember(nameof(IPluginModel.SystemName), options => options.MapFrom(plugin => ((IPlugin)plugin).PluginDescriptor.SystemName));
                        }
                    }
                });
            }

        protected virtual void CreateCatalogMaps()
        {
            CreateMap<Core.Domain.Catalog.Product, ProductDiscontinuedModel>()
                .ForMember(model => model.AddPictureModel, options => options.Ignore())
                .ForMember(model => model.AddSpecificationAttributeModel, options => options.Ignore())
                .ForMember(model => model.AssociatedProductSearchModel, options => options.Ignore())
                .ForMember(model => model.AssociatedToProductId, options => options.Ignore())
                .ForMember(model => model.AssociatedToProductName, options => options.Ignore())
                .ForMember(model => model.AvailableBasepriceBaseUnits, options => options.Ignore())
                .ForMember(model => model.AvailableBasepriceUnits, options => options.Ignore())
                .ForMember(model => model.AvailableCategories, options => options.Ignore())
                .ForMember(model => model.AvailableDeliveryDates, options => options.Ignore())
                .ForMember(model => model.AvailableManufacturers, options => options.Ignore())
                .ForMember(model => model.AvailableProductAvailabilityRanges, options => options.Ignore())
                .ForMember(model => model.AvailableProductTemplates, options => options.Ignore())
                .ForMember(model => model.AvailableTaxCategories, options => options.Ignore())
                .ForMember(model => model.AvailableVendors, options => options.Ignore())
                .ForMember(model => model.AvailableWarehouses, options => options.Ignore())
                .ForMember(model => model.BaseDimensionIn, options => options.Ignore())
                .ForMember(model => model.BaseWeightIn, options => options.Ignore())
                .ForMember(model => model.CopyProductModel, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.CrossSellProductSearchModel, options => options.Ignore())
                .ForMember(model => model.IsLoggedInAsVendor, options => options.Ignore())
                .ForMember(model => model.LastStockQuantity, options => options.Ignore())
                .ForMember(model => model.PictureThumbnailUrl, options => options.Ignore())
                .ForMember(model => model.PrimaryStoreCurrencyCode, options => options.Ignore())
                .ForMember(model => model.ProductAttributeCombinationSearchModel, options => options.Ignore())
                .ForMember(model => model.ProductAttributeMappingSearchModel, options => options.Ignore())
                .ForMember(model => model.ProductAttributesExist, options => options.Ignore())
                .ForMember(model => model.ProductEditorSettingsModel, options => options.Ignore())
                .ForMember(model => model.ProductOrderSearchModel, options => options.Ignore())
                .ForMember(model => model.ProductPictureModels, options => options.Ignore())
                .ForMember(model => model.ProductPictureSearchModel, options => options.Ignore())
                .ForMember(model => model.ProductSpecificationAttributeSearchModel, options => options.Ignore())
                .ForMember(model => model.ProductsTypesSupportedByProductTemplates, options => options.Ignore())
                .ForMember(model => model.ProductTags, options => options.Ignore())
                .ForMember(model => model.ProductTypeName, options => options.Ignore())
                .ForMember(model => model.ProductWarehouseInventoryModels, options => options.Ignore())
                .ForMember(model => model.RelatedProductSearchModel, options => options.Ignore())
                .ForMember(model => model.SelectedCategoryIds, options => options.Ignore())
                .ForMember(model => model.SelectedManufacturerIds, options => options.Ignore())
                .ForMember(model => model.SeName,
                    options => options.MapFrom(entity =>
                        EngineContext.Current.Resolve<IUrlRecordService>().GetSeName(entity, 0, true, false)))
                .ForMember(model => model.StockQuantityHistory, options => options.Ignore())
                .ForMember(model => model.StockQuantityHistorySearchModel, options => options.Ignore())
                .ForMember(model => model.StockQuantityStr, options => options.Ignore())
                .ForMember(model => model.TierPriceSearchModel, options => options.Ignore())
                .ForMember(model => model.UpdatedOn, options => options.Ignore());
            CreateMap<ProductDiscontinuedModel, Core.Domain.Catalog.Product>()
                .ForMember(entity => entity.ApprovedRatingSum, options => options.Ignore())
                .ForMember(entity => entity.ApprovedTotalReviews, options => options.Ignore())
                .ForMember(entity => entity.BackorderMode, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.Deleted, options => options.Ignore())
                .ForMember(entity => entity.DiscountProductMappings, options => options.Ignore())
                .ForMember(entity => entity.DownloadActivationType, options => options.Ignore())
                .ForMember(entity => entity.GiftCardType, options => options.Ignore())
                .ForMember(entity => entity.HasDiscountsApplied, options => options.Ignore())
                .ForMember(entity => entity.HasTierPrices, options => options.Ignore())
                .ForMember(entity => entity.LowStockActivity, options => options.Ignore())
                .ForMember(entity => entity.ManageInventoryMethod, options => options.Ignore())
                .ForMember(entity => entity.NotApprovedRatingSum, options => options.Ignore())
                .ForMember(entity => entity.NotApprovedTotalReviews, options => options.Ignore())
                .ForMember(entity => entity.ParentGroupedProductId, options => options.Ignore())
                .ForMember(entity => entity.ProductAttributeCombinations, options => options.Ignore())
                .ForMember(entity => entity.ProductAttributeMappings, options => options.Ignore())
                .ForMember(entity => entity.ProductCategories, options => options.Ignore())
                .ForMember(entity => entity.ProductManufacturers, options => options.Ignore())
                .ForMember(entity => entity.ProductPictures, options => options.Ignore())
                .ForMember(entity => entity.ProductProductTagMappings, options => options.Ignore())
                .ForMember(entity => entity.ProductReviews, options => options.Ignore())
                .ForMember(entity => entity.ProductSpecificationAttributes, options => options.Ignore())
                .ForMember(entity => entity.ProductType, options => options.Ignore())
                .ForMember(entity => entity.ProductWarehouseInventory, options => options.Ignore())
                .ForMember(entity => entity.RecurringCyclePeriod, options => options.Ignore())
                .ForMember(entity => entity.RentalPricePeriod, options => options.Ignore())
                .ForMember(entity => entity.TierPrices, options => options.Ignore())
                .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore());
        }

        public int Order { get; }
    }
}
