using Application.Interfaces;
using Application.Services.Revisions;
using Application.Services.Wiql;
using Infraestructure.Repositories;
using System.Text.Json.Serialization;
using static Domain.Enums;

namespace ApiTools
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();                 

            // Add services to the container.
            builder.Services.AddScoped<RevisionRepository>();
            builder.Services.AddScoped<WiqlRepository>();
            builder.Services.AddScoped<GetDevelopersInRevisionQueryHandler>();
            builder.Services.AddScoped<GetQueryById>();
            builder.Services.AddScoped<GetQueryByRawWiql>();
            builder.Services.AddHttpClient<IRevisionRepository, RevisionRepository>();
            builder.Services.AddHttpClient<IWiqlRepository, WiqlRepository>();

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
        }
    }
}
