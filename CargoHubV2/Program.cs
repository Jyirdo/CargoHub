using CargohubV2.Contexts;
using CargohubV2.DataConverters;
using Microsoft.EntityFrameworkCore;
using CargohubV2.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<CargoHubDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CargoHubDatabase")));

builder.Services.AddScoped<ItemGroupService>();
builder.Services.AddScoped<ItemLineService>();
builder.Services.AddScoped<ItemTypeService>();
builder.Services.AddScoped<ItemService>();
builder.Services.AddScoped<ClientsService>();
builder.Services.AddScoped<TransferService>();
builder.Services.AddScoped<WarehouseService>();
builder.Services.AddScoped<SupplierService>();
builder.Services.AddScoped<LocationService>();
builder.Services.AddScoped<InventoriesService>();
builder.Services.AddScoped<ShipmentService>();
builder.Services.AddScoped<OrderService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
if (args.Length > 0 && args[0] == "seed")
{
    SeedData1(app);
}

app.UseAuthorization();

app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers();

app.Run();

void SeedData1(IHost app)
{
    var scopedFactory = app.Services.GetServices<IServiceScopeFactory>();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        DataLoader.ImportData(services.GetRequiredService<CargoHubDbContext>());
    }
}

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string ApiKeyHeaderName = "API_KEY";
    private const string ValidApiKey = "cargohub123";

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            if (context.Request.Method != HttpMethods.Get)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("API Key is required for non-GET methods.");
                Console.WriteLine($"Unauthorized access attempt to {context.Request.Path}.");
                return;
            }
        }
        else if (extractedApiKey != ValidApiKey)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Invalid API Key.");
            Console.WriteLine($"Forbidden access attempt to {context.Request.Path} with API Key: {extractedApiKey}");
            return;
        }

        await _next(context);
    }
}
