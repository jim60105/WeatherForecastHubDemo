using WeatherForecastHub.Data;
using WeatherForecastHub.Repositories;
using WeatherForecastHub.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// 設定資料庫連線
builder.Services.AddDbContext<WeatherDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 註冊 Repository 層
builder.Services.AddScoped<ICityRepository, CityRepository>();

// 註冊 Service 層
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IWeatherService, WeatherService>();

// 註冊 HttpClient
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });
}

// 啟用靜態檔案服務
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

// 設定控制器路由前綴為 /api
app.MapControllers().RequireAuthorization().WithGroupName("api");

// 定義一個簡單的端點，將根路徑重定向到 Swagger UI
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
