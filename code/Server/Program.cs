using KillTheFly.Server.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddSingleton<GameService>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "KillTheFly API", Version = "v1" });
});

var connectionString = builder.Configuration.GetSection("ConnectionStrings:Psql").Value;
builder.Services.AddDbContext<KTFDatabaseContext>(options =>
    options.UseNpgsql(connectionString), ServiceLifetime.Singleton);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>   
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "KillTheFly API V1");
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
