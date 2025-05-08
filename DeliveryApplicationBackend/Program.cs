using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container

// Configure database connection using Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add CORS support to allow React app to communicate with the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", builder =>
    {
        builder.WithOrigins("http://localhost:3000") // React frontend URL
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Add SignalR for real-time communication
builder.Services.AddSignalR();

// Add controllers for API
builder.Services.AddControllers();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Delivery API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Delivery API v1");
    });
}

app.UseHttpsRedirection();

// Enable CORS to allow frontend to access API
app.UseCors("AllowReactApp");

// Enable routing and map controllers
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    // Map controller routes for API
    endpoints.MapControllers();

    // Optional: If you are using SignalR for real-time updates
    // endpoints.MapHub<OrderHub>("/orderHub");
});

app.Run();
