using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Globalization;

namespace HrmsApi.Localization
{
    public static class LocalizationExtensions
    {
        public static IServiceCollection AddHrmsLocalization(this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en"),
                    new CultureInfo("fr"),
                    new CultureInfo("es")
                };
                
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                
                // Add custom request culture provider that gets culture from HTTP header
                options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(context =>
                {
                    var userLangs = context.Request.Headers["Accept-Language"].ToString();
                    var firstLang = userLangs.Split(',').FirstOrDefault();
                    
                    var defaultLang = string.IsNullOrEmpty(firstLang) ? "en" : firstLang;
                    
                    return Task.FromResult(new ProviderCultureResult(defaultLang));
                }));
            });
            
            return services;
        }
    }
}
