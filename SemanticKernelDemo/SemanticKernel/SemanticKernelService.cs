using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SemanticKernelDemo.SemanticKernel;

public class SemanticKernelService(Kernel kernel, IKernelMemory memory) : ISemanticKernelService
{
    const string systemPrompt =
        """
        You are an AI assistant that helps people find information contained within documents uploaded to your memory.
        If the user asks for anything else,
        politely advise them that you can only answer questions regarding documents within your memory.
        """;

    public async Task ImportText(string text)
    {
        await memory.ImportTextAsync(text);
    }
    
    public async Task<string> Query(string query)
    {
        var result = await memory.AskAsync(query);
        return result.Result;
    }
}