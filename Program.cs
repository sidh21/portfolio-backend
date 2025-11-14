using Microsoft.OpenApi.Models;
using PortfolioBackend.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// ✅ Enable Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Portfolio Backend API",
        Version = "v1",
        Description = "API for Portfolio contact form"
    });
});

// ✅ Allow CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5174", "https://portfolio-frontend-rho-liart.vercel.app")

                   .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// ✅ Load EmailSettings from appsettings.json
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// ✅ Register EmailService
builder.Services.AddScoped<EmailService>();

var app = builder.Build();

// ✅ Use Swagger UI when running in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Portfolio API V1");
        options.RoutePrefix = "swagger";
    });
}

// ✅ Enable CORS before controllers
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "Portfolio Backend is running!");
app.Run();
