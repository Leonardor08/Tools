using System.Text.Json.Serialization;
using ApiTools.Infraestructura;
using Application;
using Infraestructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(o => o.SuppressAsyncSuffixInActionNames = false)
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ManejadorExcepciones>();

// Composición del hexágono: casos de uso (entrada) + adaptadores (salida).
builder.Services.AgregarAplicacion();
builder.Services.AgregarInfraestructura(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

public partial class Program { }
