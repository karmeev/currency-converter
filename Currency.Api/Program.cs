using Autofac;
using Autofac.Extensions.DependencyInjection;
using Currency.Api;
using Currency.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

var settings = builder.Configuration.ConfigureSettings(builder.Environment.EnvironmentName);

builder.Services.AddControllers();
builder.Services.ConfigureVersioning();
builder.Services.ConfigureIdentity(settings.InfrastructureSettings.JwtSettings);
builder.Services.AddOptions();
builder.Services.AddHttpClient();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();


builder.Host.ConfigureContainer<ContainerBuilder>(container =>
{
    Registry.RegisterDependencies(container, settings, builder.Configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();