using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using Blazored.LocalStorage;

namespace Quepland
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<GameState>();
            services.AddScoped<MessageManager>();
            services.AddScoped<Microsoft.AspNetCore.Blazor.Services.WebAssemblyUriHelper>();
            services.AddBlazoredLocalStorage();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
