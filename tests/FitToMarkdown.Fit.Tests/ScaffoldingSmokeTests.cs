using FluentAssertions;
using FitToMarkdown.Fit;
using Xunit;

namespace FitToMarkdown.Fit.Tests;

public sealed class ScaffoldingSmokeTests
{
    [Fact]
    public void FitAssemblyMarker_should_be_loadable()
    {
        var markerType = typeof(FitAssemblyMarker);

        markerType.Should().NotBeNull();
        markerType.Assembly.GetName().Name.Should().Be("FitToMarkdown.Fit");
    }
}