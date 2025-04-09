using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using HrmsApi.Modules.Employee.Infrastructure;
using HrmsApi.Modules.Employee.Domain.Interfaces;
using HrmsApi.Modules.Leave.Infrastructure;
using HrmsApi.Modules.Leave.Application.Services;
using HrmsApi.Modules.Leave.Application.Interfaces;
using HrmsApi.Modules.Leave.Domain.Interfaces;
using HrmsApi.Modules.Employee.Application.Interfaces;
using HrmsApi.Modules.Chatbot;
using HrmsApi.Modules.MoodChanger;

namespace HrmsApi.Shared.Infrastructure
{
    public static class ModuleRegistration
    {
        public static IServiceCollection RegisterModules(this IServiceCollection services, IConfiguration configuration)
        {
            // Register Employee Module
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IEmployeeHistoryRepository, EmployeeHistoryRepository>();
            services.AddScoped<IPayrollHistoryRepository, PayrollHistoryRepository>();
            
            // Register Leave Module
            services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
            services.AddScoped<ILeaveRequestService, LeaveRequestService>();
            
            // Register Attendance Module
            // services.AddAttendanceModule();
            
            // Register Settings Module
            // No repositories or services to register yet, but the controllers will be discovered automatically
            
            // Register Chatbot Module
            services.AddChatbotModule(configuration);
            
            // Register MoodChanger Module
            services.AddMoodChangerModule(configuration);
            
            return services;
        }
    }
}
