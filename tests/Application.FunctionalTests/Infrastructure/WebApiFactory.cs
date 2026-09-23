using FocusPocuss.Application.Common.Interfaces;
using FocusPocuss.Application.Tasks.Planning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FocusPocuss.Application.FunctionalTests.Infrastructure;

public class WebApiFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .UseSetting("ConnectionStrings:FocusPocussDb", connectionString);

        builder.ConfigureTestServices(services =>
        {
            services
                .RemoveAll<ITaskStartPlanner>()
                .AddSingleton<TestTaskStartPlanner>()
                .AddSingleton<ITaskStartPlanner>(provider =>
                    provider.GetRequiredService<TestTaskStartPlanner>());

            services
                .RemoveAll<IUser>()
                .AddTransient(provider =>
                {
                    var mock = new Mock<IUser>();
                    mock.SetupGet(x => x.Roles).Returns(TestApp.GetRoles());
                    mock.SetupGet(x => x.Id).Returns(TestApp.GetUserId());
                    return mock.Object;
                });
        });
    }
}
