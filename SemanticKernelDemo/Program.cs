using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel.ChatCompletion;
using SemanticKernelDemo.Database;
using SemanticKernelDemo.SemanticKernel;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddDatabaseServices();
builder.AddSemanticKernel();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.ConfigureDatabaseServices();

app.MapPost("Query",
        async (ISemanticKernelService semanticKernelService, string query) =>
        {
            var result = await semanticKernelService.Query(query);
            return TypedResults.Ok(result);
        })
    .WithOpenApi(operation =>
    {
        operation.Summary = "Query Transcriptions";
        operation.Description = "Query Transcriptions";
        operation.Tags = new List<OpenApiTag> { new() { Name = "AI" } };

        return operation;
    });

app.MapPost("/upload",
        async (IFormFile file) =>
        {
            await using var stream = file.OpenReadStream();
            var buffer = new byte[stream.Length];
            await stream.ReadAsync(buffer);
            var text = Encoding.UTF8.GetString(buffer);

            var semanticKernelService = app.Services.GetRequiredService<ISemanticKernelService>();
            await semanticKernelService.ImportText(text);

            return TypedResults.Ok();
        })
    .DisableAntiforgery();

app.Run();

