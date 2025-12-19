using MatrixN3HealthManager.Main;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IN3HealthService, N3HealthManager>();

var app = builder.Build();

// Отключаем HTTPS редирект для работы через LoadBalancer без SSL
// app.UseHttpsRedirection();

// Включаем Swagger в Production для доступа к API документации
    app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "N3 Health Manager API v1");
    c.RoutePrefix = "swagger"; // Swagger доступен по /swagger
});

app.UseAuthorization();

app.MapControllers();

app.Run();