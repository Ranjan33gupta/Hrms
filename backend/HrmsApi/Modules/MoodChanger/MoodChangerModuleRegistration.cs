using HrmsApi.Modules.MoodChanger.Application.Services;
using HrmsApi.Modules.MoodChanger.Infrastructure.Data;
using HrmsApi.Modules.MoodChanger.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HrmsApi.Modules.MoodChanger
{
    public static class MoodChangerModuleRegistration
    {
        public static IServiceCollection AddMoodChangerModule(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<MoodChangerDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            
            // Register repositories
            services.AddScoped<MoodEntryRepository>();
            
            // Register services
            services.AddScoped<MoodAnalysisService>();
            
            return services;
        }
    }
}
