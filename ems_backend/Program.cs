using Microsoft.EntityFrameworkCore;
using ems_backend.Data;
using ems_backend.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//CORS
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

//Dependency Injection
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<EventCategoryService>();
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<DiscussionService>();

builder.Services.AddSingleton<CloudinaryService>();

// Configure Serilog for file logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/logfile.txt", rollingInterval: RollingInterval.Day)  // Creates a new log file every day
    .CreateLogger();
builder.Host.UseSerilog(); // Replace default logging with Serilog

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application started");