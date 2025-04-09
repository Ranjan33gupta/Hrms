using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HrmsApi.Validation
{
    public static class ValidationExtensions
    {
        public static IServiceCollection AddHrmsValidation(this IServiceCollection services)
        {
            // Add FluentValidation
            services.AddFluentValidationAutoValidation();
            
            // Register all validators from the assembly
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            
            return services;
        }
    }
}
