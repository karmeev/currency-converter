using Autofac;
using Autofac.Extensions.DependencyInjection;
using Currency.Api;
using Currency.Api.BackgroundServices;
using Currency.Api.Configurations;
using Currency.Api.Middlewares;
using Currency.Api.ModelBinders;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Services.AddSettings(builder.Environment.EnvironmentName, out var settings);
builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new CustomModelBinderProvider());
});
builder.Services.AddVersioning();
builder.Services.AddRateLimiter(settings);
builder.Services.AddIdentity(settings);
builder.Services.AddCustomBehavior();
builder.Services.AddThirdPartyApis(settings);
builder.Services.AddHostedService<ConsumersStartupBackgroundService>();

builder.Host.AddLogger(builder.Services, settings);
builder.Host.ConfigureContainer<ContainerBuilder>(Registry.RegisterDependencies);

var app = builder.Build();
app.UseHttpsRedirection();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();
app.Run();