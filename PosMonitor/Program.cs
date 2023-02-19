using Microsoft.AspNetCore.ResponseCompression;
using PosMonitor.Service;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.Services.AddSyncfusionBlazor();
builder.Services.AddSingleton<PositionsMonitor>();
builder.Services.AddHostedService<PositionsMonitor>((s) => s.GetRequiredService<PositionsMonitor>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapHub<PositionHub>("/positionsupdatehub");
app.MapFallbackToPage("/_Host");

app.Run();

