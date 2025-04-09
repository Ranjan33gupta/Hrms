using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HrmsApi.Data;
using HrmsApi.Services;
using HrmsApi.Middleware;
using HrmsApi.Shared.Infrastructure;
using HrmsApi.Commands;
using HrmsApi.Utils;
using HrmsApi.Logging;
using HrmsApi.Validation;
using HrmsApi.Modules.Chatbot.Infrastructure;
using Serilog;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog();

// Check for command-line arguments
if (args.Length > 0 && args[0].Equals("generate-migration", StringComparison.OrdinalIgnoreCase))
{
    // Handle schema migration generation
    await RunSchemaGeneration(builder.Configuration);
    return;
}

// Add services to the container.
builder.Services.AddHrmsLogging();
builder.Services.AddHrmsValidation();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HRMS API",
        Version = "v1",
        Description = "API for Human Resource Management System"
    });

    // Configure Swagger to use JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Register Migration Command
builder.Services.AddScoped<GenerateMigrationCommand>();

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "HrmsApi",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "HrmsClient",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"] ?? "HrmsDefaultSecretKeyForDevelopment12345"))
        };
    });

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register services
builder.Services.AddTransient<AuthService>();

// Register all modules
builder.Services.RegisterModules(builder.Configuration);

// Always configure PostgreSQL for consistency with migrations
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection string: {connectionString}");

var useInMemoryDb = string.IsNullOrEmpty(connectionString);

if (useInMemoryDb)
{
    // Use in-memory database only if connection string is not provided
    builder.Services.AddDbContext<HrmsApi.Data.HrmsDbContext>(options =>
        options.UseInMemoryDatabase("HrmsDb"));
    Console.WriteLine("Using in-memory database");
}
else
{
    // Use PostgreSQL
    // Configure Npgsql with proper JSON serialization
    builder.Services.AddDbContext<HrmsApi.Data.HrmsDbContext>(options =>
    {
        // Configure connection pooling
        var connectionStringBuilder = new Npgsql.NpgsqlConnectionStringBuilder(connectionString)
        {
            MaxPoolSize = 50,      // Maximum number of connections in the pool
            MinPoolSize = 5,       // Minimum number of connections in the pool
            ConnectionIdleLifetime = 300,  // Connection idle lifetime in seconds
            ConnectionPruningInterval = 10, // How often to check for idle connections to prune
            Pooling = true         // Enable connection pooling
        };

        // Create a data source builder with dynamic JSON enabled and optimized connection pooling
        var dataSourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(connectionStringBuilder.ConnectionString);
        dataSourceBuilder.EnableDynamicJson();

        // Use the data source builder in the DbContext options with optimized connection pooling
        options.UseNpgsql(dataSourceBuilder.Build(), npgsqlOptions =>
            npgsqlOptions
                .EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null)
                .CommandTimeout(30)
                .MaxBatchSize(100)
                .MigrationsHistoryTable("__EFMigrationsHistory")
                .SetPostgresVersion(15, 0))
              .ConfigureWarnings(warnings =>
                  warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning))
              // Optimize connection pooling
              .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
              .EnableDetailedErrors(builder.Environment.IsDevelopment());
    });
    Console.WriteLine("Using PostgreSQL database");
}

// Register Auth Service
builder.Services.AddTransient<HrmsApi.Services.AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use global exception middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAll");

app.UseMiddleware<JwtMiddleware>();

app.UseRouting();

// Add authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Create and migrate the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<HrmsApi.Data.HrmsDbContext>();

        if (useInMemoryDb)
        {
            // For in-memory database, just ensure it's created
            context.Database.EnsureCreated();
            Console.WriteLine("In-memory database created successfully");
        }
        else
        {
            try
            {
                // Test database connection
                var canConnect = context.Database.CanConnect();
                Console.WriteLine($"Can connect to database: {canConnect}");

                if (canConnect)
                {
                    // For PostgreSQL, apply migrations
                    context.Database.Migrate();
                    Console.WriteLine("PostgreSQL database migrated successfully");

                    // Run the schema migration generator to detect and generate migration scripts
                    // for any entity model changes
                    await MigrationRunner.RunMigrationGenerator(services);

                    // Initialize Chatbot database tables
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    ChatbotDbInitializer.Initialize(services, logger);

                    // Check if tables exist and have data
                    var employeeCount = context.Employees.Count();
                    var userCount = context.Users.Count();
                    var leaveRequestCount = context.LeaveRequests.Count();

                    Console.WriteLine($"Database tables information:");
                    Console.WriteLine($"- Employees table: {employeeCount} records");
                    Console.WriteLine($"- Users table: {userCount} records");
                    Console.WriteLine($"- LeaveRequests table: {leaveRequestCount} records");

                    // List all tables in the database
                    var tables = context.Database.SqlQuery<string>($"SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'").ToList();
                    Console.WriteLine($"Tables in database:");
                    foreach (var table in tables)
                    {
                        Console.WriteLine($"- {table}");
                    }
                }
                else
                {
                    Console.WriteLine("Cannot connect to the PostgreSQL database. Please check your connection string and ensure PostgreSQL is running.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to database: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating/migrating the database.");
        Console.WriteLine($"Database error: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
}

// Create uploads directory for attendance photos
string uploadsDir = Path.Combine(app.Environment.ContentRootPath, "Uploads", "AttendancePhotos");
if (!Directory.Exists(uploadsDir))
{
    Directory.CreateDirectory(uploadsDir);
    Console.WriteLine($"Created uploads directory at: {uploadsDir}");
}

app.Run();

// Helper method to run schema migration generation
async Task RunSchemaGeneration(IConfiguration configuration)
{
    Console.WriteLine("== HRMS Schema Migration Generator ==");
    Console.WriteLine("Generating database migration scripts based on entity models...");

    try
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            Console.WriteLine("Error: No database connection string provided.");
            return;
        }

        // Create migrations directory if it doesn't exist
        var migrationsPath = Path.Combine(Directory.GetCurrentDirectory(), "Migrations");
        if (!Directory.Exists(migrationsPath))
        {
            Directory.CreateDirectory(migrationsPath);
        }

        var generator = new SchemaMigrationGenerator(connectionString, migrationsPath);
        await generator.GenerateMigrationScript();

        Console.WriteLine("Migration script generation completed.");
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error generating migration: {ex.Message}");
        Console.WriteLine(ex.StackTrace);
        Console.ResetColor();
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
