using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using RestroMenu.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( c =>
{
    c.CustomSchemaIds((type) => type.FullName);
    //c.OperationFilter<XAuthHeaderFilter>();
    c.DocumentFilter<SwaggerDocumentFilter>();

    c.AddSecurityDefinition(name: "Token", securityScheme: new OpenApiSecurityScheme
    {
        Name = "X-Auth",
        Description = "Enter token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "Token",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Id = "Token",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

builder.Services.Configure<DatabaseSetting>(builder.Configuration.GetSection(nameof(DatabaseSetting)));

builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
{
    DatabaseSetting settings = sp.GetRequiredService<IOptions<DatabaseSetting>>().Value;

    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    DatabaseSetting settings = sp.GetRequiredService<IOptions<DatabaseSetting>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();

    return client.GetDatabase(settings.DatabaseName);
});

builder.Services.AddSingleton<DbHelper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
