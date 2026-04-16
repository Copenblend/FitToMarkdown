using FluentAssertions;
using FitToMarkdown.Fit.Mapping;
using Xunit;

namespace FitToMarkdown.Fit.Tests.Mapping;

public sealed class FitMessageIdentityFactoryTests
{
    [Fact]
    public void Create_WithDefaults_ReturnsExpectedIdentity()
    {
        var identity = FitMessageIdentityFactory.Create(parseSequence: 5);

        identity.ParseSequence.Should().Be(5);
        identity.MessageIndex.Should().BeNull();
        identity.IsSynthetic.Should().BeFalse();
        identity.RecoveredAfterDecodeFault.Should().BeFalse();
    }

    [Fact]
    public void Create_WithAllParams_SetsAllProperties()
    {
        var identity = FitMessageIdentityFactory.Create(
            parseSequence: 10,
            messageIndex: 3,
            isSynthetic: true,
            recoveredAfterFault: true);

        identity.ParseSequence.Should().Be(10);
        identity.MessageIndex.Should().Be(3);
        identity.IsSynthetic.Should().BeTrue();
        identity.RecoveredAfterDecodeFault.Should().BeTrue();
    }

    [Fact]
    public void Create_AsSynthetic_MarksIdentity()
    {
        var identity = FitMessageIdentityFactory.Create(parseSequence: 0, isSynthetic: true);

        identity.IsSynthetic.Should().BeTrue();
        identity.RecoveredAfterDecodeFault.Should().BeFalse();
    }
}
