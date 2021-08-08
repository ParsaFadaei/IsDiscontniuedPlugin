using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Product.Discontinued.Data;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Product.Discontinued.Infrastructure
{
   
    public class PluginDbStartup : INopStartup
    {
        
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //add object context
            services.AddDbContext<DiscontinuedObjectContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServerWithLazyLoading(services);
            });
        }

        public void Configure(IApplicationBuilder application)
        {
        }

        public int Order => 11;
    }
}