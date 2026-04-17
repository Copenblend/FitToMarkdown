using FluentAssertions;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Parsing;

namespace FitToMarkdown.Fit.Tests.Fixtures;

/// <summary>
/// Centralizes invariant assertions for success, partial success, and fatal parser envelopes.
/// </summary>
internal static class FitParseAssertions
{
    internal static void AssertSucceeded(FitParseResult result)
    {
        result.Should().NotBeNull("parse result should always be returned");
        result.Document.Should().NotBeNull("a successful parse must produce a document");
        result.FatalError.Should().BeNull("a successful parse must not have a fatal error");
        result.Metadata.Status.Should().Be(FitParseStatus.Succeeded);
        result.Metadata.IsPartial.Should().BeFalse();
    }

    internal static void AssertPartiallySucceeded(FitParseResult result)
    {
        result.Should().NotBeNull("parse result should always be returned");
        result.Document.Should().NotBeNull("a partial parse must still produce a document");
        result.FatalError.Should().BeNull("a partial success must not have a fatal error");
        result.Metadata.Status.Should().Be(FitParseStatus.PartiallySucceeded);
        result.Metadata.IsPartial.Should().BeTrue();
    }

    internal static void AssertFailed(FitParseResult result, string expectedCode)
    {
        result.Should().NotBeNull("parse result should always be returned");
        result.Document.Should().BeNull("a fatal parse failure must not produce a document");
        result.FatalError.Should().NotBeNull("a fatal failure must carry a fatal error");
        result.FatalError!.Code.Should().Be(expectedCode);
        result.Metadata.Status.Should().Be(FitParseStatus.Failed);
    }

    internal static void AssertMetadataWindow(FitParseMetadata metadata)
    {
        metadata.Should().NotBeNull("metadata must always be populated");
        metadata.DecodedMessageCount.Should().BeGreaterOrEqualTo(0);
        metadata.DroppedMessageCount.Should().BeGreaterOrEqualTo(0);
    }

    internal static void AssertCollectionsNotNull(FitParseResult result)
    {
        result.Issues.Should().NotBeNull("issues collection must never be null");
        if (result.Document is not null)
        {
            result.Document.DeviceInfos.Should().NotBeNull("device infos collection must never be null");
            result.Document.DeveloperDataIds.Should().NotBeNull("developer data ids must never be null");
            result.Document.FieldDescriptions.Should().NotBeNull("field descriptions must never be null");
        }
    }
}
