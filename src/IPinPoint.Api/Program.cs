var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseSwagger();
app.UseSwaggerUI();

app.Run();

public partial class Program { }