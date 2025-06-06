using Microsoft.Extensions.DependencyInjection;
using RealtimeX.Dashboard.Services;
using RealtimeX.Dashboard.Models;

namespace RealtimeX.Dashboard.API
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // MongoDB Configuration
            services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDbSettings"));
            services.AddScoped<IUnitOfWork, MongoUnitOfWork>();
        }
    }
} 