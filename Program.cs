using HNTopAPI.Globals;

await Globals.GetItems();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddResponseCompression();


var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseAuthorization();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseResponseCompression();

app.MapControllers();

app.Run();
