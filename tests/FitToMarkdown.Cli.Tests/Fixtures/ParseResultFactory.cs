using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Parsing;

namespace FitToMarkdown.Cli.Tests.Fixtures;

internal static class ParseResultFactory
{
    internal static FitParseResult CreateSuccessful(string? inputPath = null)
    {
        return new FitParseResult
        {
            Document = new FitFileDocument(),
            Metadata = new FitParseMetadata(),
        };
    }

    internal static FitParseResult CreateFailed(string message)
    {
        return new FitParseResult
        {
            FatalError = new FitParseError
            {
                Code = "PARSE_FAILED",
                Message = message,
            },
        };
    }
}
