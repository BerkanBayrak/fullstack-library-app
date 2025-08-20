using LibraryApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (builder.Environment.IsDevelopment())
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        else
            policy.WithOrigins("https://lively-wonder-463006-q6.web.app")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
    });
});

// Controllers + enum as string + camelCase
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// ---------- Database (MySQL if env var set, else SQLite) ----------
var conn = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
if (!string.IsNullOrWhiteSpace(conn))
{
    // Oracle provider
    builder.Services.AddDbContext<LibraryDbContext>(
        opts => opts.UseMySQL(conn));
}
else
{
    var dbPath = builder.Environment.IsDevelopment() ? "library.db" : "/tmp/library.db";
    builder.Services.AddDbContext<LibraryDbContext>(
        opts => opts.UseSqlite($"Data Source={dbPath}"));
}

// -------------------------------------------------------------------

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// CORS before routing
app.UseCors("AllowFrontend");

// Swagger only in dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Ensure DB exists (switch to Migrate() when you add EF migrations)
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    ctx.Database.EnsureCreated();
}

app.MapControllers();
app.MapGet("/", () => Results.Content("<h1>📚 Library API is running</h1>", "text/html"));
app.Run();
