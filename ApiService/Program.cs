using System.Reflection;
using ApiService.Python;
using Data;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
builder.Services.AddProblemDetails();
builder.Services.AddCors();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});
builder.Services.AddEndpointsApiExplorer();

// Add database context for user input parsing
builder.AddSqlServerDbContext<UserInputDbContext>("userdetailsdb");

builder.Services.AddOpenApiDocument(options =>
{
    options.DocumentName = "v1";
    options.Title = "User Details API"; // updated title
    options.Version = "v1";
    options.UseHttpAttributeNameAsOperationId = true;
    options.PostProcess = document => { document.BasePath = "/"; };
});

// Keep calling the Python API (which now talks to Groq internally)
builder.Services.AddHttpClient<PythonClient>(
    static client => client.BaseAddress = new("http://pythonapi"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseCors(static cors =>
{
    cors.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithExposedHeaders("*");
});

app.MapDefaultEndpoints();
app.MapControllers();
app.UseOpenApi();
app.UseSwaggerUi();
app.Run();
