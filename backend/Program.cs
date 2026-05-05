using HelpDeskFlow.Repositories;
using HelpDeskFlow.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton(typeof(IJsonRepository<>), typeof(JsonRepository<>));
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<DashboardService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("Frontend");
app.UseAuthorization();

app.MapGet("/", () => new
{
    name = "HelpDeskFlow API",
    version = "1.0.0",
    status = "Running"
});

app.MapControllers();

app.Run();
