using HomeControl.Application.InterFaces;
using HomeControl.Infrastructure.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string baseUrl = builder.Configuration["HomeAssistant:BaseUrl"];
string token = builder.Configuration["HomeAssistant:Token"];

builder.Services.AddHttpClient<IHomeAssistantClient, HomeAssistantClient>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
});
//builder.Services.AddScoped<IHomeAssistantClient, HomeAssistantClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
