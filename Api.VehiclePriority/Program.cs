// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Auditing.Extensions;
using Econolite.Ode.Authorization.Extensions;
using Econolite.Ode.Domain.SystemModeller;
using Econolite.Ode.Domain.VehiclePriority;
using Econolite.Ode.Messaging.Extensions;
using Econolite.Ode.Persistence.Mongo;
using Econolite.Ode.Repository.VehiclePriority;
using Econolite.Ode.Monitoring.HealthChecks.Mongo.Extensions;
using Econolite.Ode.Monitoring.HealthChecks.Kafka.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Econolite.Ode.Domain.SystemModeller.Extensions;
using Econolite.Ode.Domain.VehiclePriority.Extensions;
using Econolite.Ode.Monitoring.Events.Extensions;
using Econolite.Ode.Monitoring.Metrics.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using ProtoBuf.Meta;

var builder = WebApplication.CreateBuilder(args);
var origins = builder.Configuration["CORSOrigins"] ?? throw new NullReferenceException("CORSOrigins missing in config");
Audit.Core.Configuration.Setup()
    .UseCustomProvider(new AuditMongoDataProvider(config => config
        .ConnectionString(builder.Configuration.GetConnectionString("Mongo"))
        .Database(builder.Configuration["Mongo:DbName"] ?? 
                  throw new NullReferenceException("Mongo:DbName missing in config"))
        .Collection(builder.Configuration["Collections:Audit"] ??
                    throw new NullReferenceException("Collections:Audit missing in config"))
        // This is important!
        .SerializeAsBson(true)
    ));

// Add services to the container.
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddAudit();
builder.Services.AddLogging();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTokenHandler(options =>
{
    options.Authority = builder.Configuration["Authentication:Authority"] ??
                        throw new NullReferenceException("Authentication:Authority missing in config");
    options.ClientId = builder.Configuration["Authentication:ClientId"] ??
                       throw new NullReferenceException("Authentication:ClientId missing in config");
    options.ClientSecret = builder.Configuration["Authentication:ClientSecret"] ?? 
                           throw new NullReferenceException("Authentication:ClientSecret missing in config");
});
builder.Services.AddMongo();
builder.Services.AddMessaging();
builder.Services.AddMetrics(builder.Configuration, "Vehicle Priority Api")
    .AddUserEventSupport(builder.Configuration, _ =>
    {
        _.DefaultSource = "Vehicle Priority Api";
        _.DefaultLogName = Econolite.Ode.Monitoring.Events.LogName.SystemEvent;
        _.DefaultCategory = Econolite.Ode.Monitoring.Events.Category.Server;
        _.DefaultTenantId = Guid.Empty;
    });
builder.Services.AddPriorityConfigResponseWorker(
    options => options.DefaultChannel = builder.Configuration["Topics:ConfigPriorityResponse"] ?? throw new NullReferenceException("Topics:ConfigPriorityResponse missing in config"),
    options => options.DefaultChannel = builder.Configuration["Topics:ConfigIntersectionRequest"] ?? throw new NullReferenceException("Topics:ConfigIntersectionRequest missing in config")
);
builder.Services.AddSystemModellerService();
builder.Services.AddVehiclePriorityRepos();
builder.Services.AddVehiclePriority();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins(origins?.Split(';') ?? new[] { "*" })
                .AllowCredentials();
        });
});

builder.Services.AddMvc(config =>
{
    config.Filters.Add(new AuthorizeFilter());
});

builder.Services.AddAuthenticationJwtBearer(builder.Configuration, builder.Environment.IsDevelopment());

builder.Services.AddSwaggerGen(c =>
{
#if DEBUG
    var basePath = AppDomain.CurrentDomain.BaseDirectory;
    var fileName = typeof(Program).GetTypeInfo().Assembly.GetName().Name + ".xml";
    c.IncludeXmlComments(Path.Combine(basePath, fileName));
#endif
    
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
    });
                
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme,
                },
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header,
            },
            new List<string>()
        },
    });
});

builder.Services.AddHealthChecks()
    .AddProcessAllocatedMemoryHealthCheck(maximumMegabytesAllocated: 1024, name: "Process Allocated Memory", tags: new[] { "memory" })
    .AddKafkaHealthCheck()
    .AddMongoDbHealthCheck();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
} else if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseCors();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseHealthChecksPrometheusExporter("/metrics");
app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/healthz", new HealthCheckOptions()
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
    endpoints.MapControllers();
    endpoints.MapHub<VehiclePriorityVehicleStatusHub>("vehicleStatusHub");
});

app.Run();
