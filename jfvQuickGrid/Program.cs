using jfvQuickGrid.Components;
using jfvQuickGrid.Services;
using Syncfusion.Blazor;
using Syncfusion.Licensing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddSingleton<IProductService, JsonProductService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddSingleton<NotificationService>();

var app = builder.Build();

var syncfusionLicenseKey = builder.Configuration["SYNCFUSION_LICENSE_KEY"];
if (string.IsNullOrWhiteSpace(syncfusionLicenseKey))
{
    app.Logger.LogWarning("Syncfusion license key is not configured. Set the SYNCFUSION_LICENSE_KEY environment variable.");
}
else
{
    SyncfusionLicenseProvider.RegisterLicense(syncfusionLicenseKey);
    app.Logger.LogInformation("Syncfusion license key was found and registered.");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseStaticFiles();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
