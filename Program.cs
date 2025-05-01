using System.Net.Http.Headers;
using WeatherForecastHub.Data;
using WeatherForecastHub.Repositories;
using WeatherForecastHub.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddHttpClient(name: "CWA",
                               httpClient =>
                               {
                                   httpClient.BaseAddress = new Uri(builder.Configuration.GetValue<string>("CWAApi:BaseUrl")
                                                                    ?? "https://opendata.cwa.gov.tw/api/v1/");

                                   httpClient.DefaultRequestHeaders.Accept.Clear();
                                   httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                               });

WebApplication app = builder.Build();

// 確保資料庫和資料表存在
using (IServiceScope scope = app.Services.CreateScope())
{
    WeatherDbContext dbContext = scope.ServiceProvider.GetRequiredService<WeatherDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint(url: "/openapi/v1.json", name: "OpenAPI V1"); });
}

// 啟用靜態檔案服務
app.UseStaticFiles();

app.UseHttpsRedirection();

// 設定控制器路由前綴為 /api
app.MapControllers().WithGroupName("api");

// 定義一個簡單的端點，將根路徑重定向到我們的前端頁面
app.MapGet(pattern: "/", () => Results.Redirect("/index.html"));

// 保留 Swagger 路由
app.MapGet(pattern: "/swagger", () => Results.Redirect("/swagger/index.html"));

app.Run();
