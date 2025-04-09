using HrmsApi.Modules.Chatbot.Application.Services;
using HrmsApi.Modules.Chatbot.Infrastructure.Data;
using HrmsApi.Modules.Chatbot.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HrmsApi.Modules.Chatbot
{
    public static class ChatbotModuleRegistration
    {
        public static IServiceCollection AddChatbotModule(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<ChatbotDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            
            // Register repositories
            services.AddScoped<ChatbotIntentRepository>();
            services.AddScoped<ChatbotConversationRepository>();
            
            // Register services
            services.AddScoped<ChatbotService>();
            services.AddScoped<IntentRecognitionService>();
            services.AddScoped<ChatbotDbInitializer>();
            
            return services;
        }
    }
}
