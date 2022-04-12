using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebUi;
using WebUi.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<FileReaderService>();
builder.Services.AddSingleton<DisputeManipulatorService>(services =>
{
    var fileReaderService = services.GetRequiredService<FileReaderService>();
    var disputeManipulatorService = new DisputeManipulatorService();

    fileReaderService.Subscribe(disputeManipulatorService);

    return disputeManipulatorService;
});
builder.Services.AddSingleton<DisputeWorkerManagerService>(services =>
{
    var disputeManipulatorService = services.GetRequiredService<DisputeManipulatorService>();
    var fileReaderService = services.GetRequiredService<FileReaderService>();
    var disputeWorkerManagerService = new DisputeWorkerManagerService(fileReaderService);

    disputeManipulatorService.Subscribe(disputeWorkerManagerService);

    return disputeWorkerManagerService;
});

await builder.Build().RunAsync();
