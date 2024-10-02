using Microsoft.SemanticKernel.ChatCompletion;

namespace SemanticKernelDemo.SemanticKernel;

public interface ISemanticKernelService
{
    Task ImportText(string text);
    Task<string> Query(string query);
}
