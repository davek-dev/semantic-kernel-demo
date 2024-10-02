using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using SemanticKernelDemo.Database;

namespace SemanticKernelDemo.SemanticKernel;

public static class SemanticKernelExtensions
{
    private const string AzureOpenAiEndpoint = "**Your Azure OpenAI endpoint**";
    private const string AzureOpenAiApiKey = "**Your Azure OpenAI API key**";
    private const string AzureOpenAiChatCompletionDeployment = "**Your Azure OpenAI deployment**";
    private const string AzureOpenAiEmbeddingDeployment = "**Your Azure OpenAI deployment**";

    public static void AddSemanticKernel(this WebApplicationBuilder builder)
    {
        var memory = new KernelMemoryBuilder()
            .WithAzureOpenAITextGeneration(new()
            {
                Deployment = AzureOpenAiChatCompletionDeployment,
                Endpoint = AzureOpenAiEndpoint,
                APIKey = AzureOpenAiApiKey,
                Auth = AzureOpenAIConfig.AuthTypes.APIKey,
                EmbeddingDimensions = 1536
            })
            .WithAzureOpenAITextEmbeddingGeneration(new()
            {
                Deployment = AzureOpenAiEmbeddingDeployment,
                Endpoint = AzureOpenAiEndpoint,
                APIKey = AzureOpenAiApiKey,
                Auth = AzureOpenAIConfig.AuthTypes.APIKey,
                EmbeddingDimensions = 1536
            }).WithSearchClientConfig(new()
            {
                EmptyAnswer =
                    "I'm sorry, I haven't found any relevant information that can be used to answer your question",
                MaxMatchesCount = 25,
                AnswerTokens = 800
            })
            .WithCustomTextPartitioningOptions(new()
            {
                // Defines the properties that are used to split the documents in chunks.
                MaxTokensPerParagraph = 1000,
                MaxTokensPerLine = 300,
                OverlappingTokens = 100
            })
            .WithPostgresMemoryDb(new PostgresConfig
            {
                ConnectionString = DbConstants.ConnectionString
            })
            .Build();

        var kernelBuilder = Kernel.CreateBuilder();

        kernelBuilder.Services.AddAzureOpenAIChatCompletion(AzureOpenAiChatCompletionDeployment, AzureOpenAiEndpoint,
            AzureOpenAiApiKey);

        var kernel = kernelBuilder.Build();

        var plugin = new MemoryPlugin(memory, "kernelMemory");
        kernel.ImportPluginFromObject(plugin, "memory");

        builder.Services.AddSingleton(kernel);
        builder.Services.AddSingleton(memory);
        builder.Services.AddTransient<ISemanticKernelService, SemanticKernelService>();
    }
}
