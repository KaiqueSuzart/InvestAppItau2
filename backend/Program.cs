using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using InvestApp.Data;
using InvestApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Adicionar CORS para permitir requisições do frontend React em localhost:5173
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configurar serviços
builder.Services.AddScoped<IRepository, DapperRepository>();
builder.Services.AddScoped<IInvestService, InvestService>();
builder.Services.AddScoped<IPrecoMedioService, PrecoMedioService>();
builder.Services.AddScoped<IPosicaoService>(sp =>
    new PosicaoService(
        sp.GetRequiredService<IRepository>(),
        sp.GetRequiredService<ICotacaoService>(),
        builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")
    ));
builder.Services.AddHttpClient<ICotacaoService, CotacaoService>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

var app = builder.Build();

// Habilitar CORS para todas as rotas, ANTES de qualquer outro middleware
app.UseCors("AllowFrontend");

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run(); 