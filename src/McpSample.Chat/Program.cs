using McpSample.BlazorChat.Components;
using Microsoft.Extensions.AI;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;

var builder = WebApplication.CreateBuilder(args);

// *************************************************************************

// Set the API service name from the Aspire configuration
string apiServiceName = "aspnetsseserver";

// *************************************************************************

// add aspire service defaults
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add configuration service
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Add logging service
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

builder.Services.AddSingleton<ILogger>(sp => sp.GetRequiredService<ILogger<Program>>());

builder.Services.AddSingleton<IMcpClient>(sp =>
{
    McpClientOptions mcpClientOptions = new()
    { ClientInfo = new() { Name = "AspNetCoreSseClient", Version = "1.0.0" } };

    var client = new HttpClient
    {
        BaseAddress = new($"https+http://{apiServiceName}")
    };

    // can't use the service discovery for ["https +http://aspnetsseserver"]
    // fix: read the environment value for the key 'services__aspnetsseserver__https__0' to get the url for the aspnet core sse server    
    var name = $"services__{apiServiceName}__https__0";
    var baseUrl = Environment.GetEnvironmentVariable(name);
    var url =  baseUrl + "/sse";

    SseClientTransportOptions sseTransportOptions = new()
    {
        Endpoint = new Uri(url)
        // Endpoint = client.BaseAddress
    };

    SseClientTransport sseClientTransport = new(transportOptions: sseTransportOptions);

    var mcpClient = McpClientFactory.CreateAsync(
        sseClientTransport, mcpClientOptions).GetAwaiter().GetResult();
    return mcpClient;
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
