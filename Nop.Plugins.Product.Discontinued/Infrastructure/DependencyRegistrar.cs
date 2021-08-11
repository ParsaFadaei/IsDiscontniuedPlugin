using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Product.Discontinued.Data;
using Nop.Plugin.Product.Discontinued.Domain;
using Nop.Plugin.Product.Discontinued.Factories;
using Nop.Plugin.Product.Discontinued.Factories.Customer;
using Nop.Plugin.Product.Discontinued.Services;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Factories;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Product.Discontinued.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {


        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<PluginProductModelFactory>().As<IPluginProductModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ProductCustomerModelFactory>().As<IProductCustomerModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<DiscontinuedServices>().As<IDiscontinuedService>().InstancePerLifetimeScope();
            builder.RegisterType<ExtendedProductService>().As<IExtendedProductService>().InstancePerLifetimeScope();

            //data context
            builder.RegisterPluginDataContext<DiscontinuedObjectContext>("nop_object_context_product_discontinued");

            //override required repository with our custom context
            builder.RegisterType<EfRepository<DiscontinuedStatus>>()
                .As<IRepository<DiscontinuedStatus>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_product_discontinued"))
                .InstancePerLifetimeScope();

        }

        public int Order => 1;
    }
}

