using Markdig;

namespace FitToMarkdown.Markdown.Validation;

internal static class MarkdownPipelineFactory
{
    internal static MarkdownPipeline CreateValidationPipeline()
    {
        return new MarkdownPipelineBuilder()
            .UseYamlFrontMatter()
            .UsePipeTables()
            .Build();
    }
}
