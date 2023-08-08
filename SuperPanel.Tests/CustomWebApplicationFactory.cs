using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using SuperPanel.App;
using Xunit;

namespace SuperPanel.Tests
{
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup: class
    {
        public string DefaultUserId { get; set; } = "1";
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                
                services.AddMvc(options =>
                    {
                        options.Filters.Add(new AllowAnonymousFilter());
                    })
                    .AddApplicationPart(typeof(Program).Assembly);
            });
        }
    }
}