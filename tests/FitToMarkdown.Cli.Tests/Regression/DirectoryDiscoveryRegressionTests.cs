using FitToMarkdown.Cli.Services;
using FitToMarkdown.Cli.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Regression;

public sealed class DirectoryDiscoveryRegressionTests
{
    private readonly FakeCliFileSystem _fileSystem;
    private readonly FitFileDiscoveryService _discoveryService;

    public DirectoryDiscoveryRegressionTests()
    {
        _fileSystem = new FakeCliFileSystem();
        _discoveryService = new FitFileDiscoveryService(_fileSystem);
    }

    [Fact]
    public async Task Empty_directory_returns_no_files()
    {
        _fileSystem.AddDirectory(@"C:\empty");

        var files = await _discoveryService.DiscoverAsync(@"C:\empty", CancellationToken.None);

        files.Should().BeEmpty();
    }

    [Fact]
    public async Task Directory_with_non_fit_files_only_returns_no_files()
    {
        _fileSystem.AddDirectory(@"C:\data");
        _fileSystem.Files[@"C:\data\readme.txt"] = new byte[100];
        _fileSystem.Files[@"C:\data\photo.jpg"] = new byte[200];
        _fileSystem.Files[@"C:\data\config.json"] = new byte[50];

        var files = await _discoveryService.DiscoverAsync(@"C:\data", CancellationToken.None);

        files.Should().BeEmpty();
    }

    [Fact]
    public async Task Mixed_case_fit_extensions_are_both_discovered()
    {
        _fileSystem.AddFitFile(@"C:\data\lower.fit");
        _fileSystem.AddFitFile(@"C:\data\upper.FIT");
        _fileSystem.AddFitFile(@"C:\data\mixed.Fit");

        var files = await _discoveryService.DiscoverAsync(@"C:\data", CancellationToken.None);

        files.Should().HaveCount(3);
        files.Select(f => f.FileName).Should().Contain("lower.fit");
        files.Select(f => f.FileName).Should().Contain("upper.FIT");
        files.Select(f => f.FileName).Should().Contain("mixed.Fit");
    }

    [Fact]
    public async Task Nested_subdirectories_are_discovered()
    {
        _fileSystem.AddFitFile(@"C:\data\activity.fit");
        _fileSystem.AddFitFile(@"C:\data\sub1\morning.fit");
        _fileSystem.AddFitFile(@"C:\data\sub1\sub2\deep.fit");

        var files = await _discoveryService.DiscoverAsync(@"C:\data", CancellationToken.None);

        files.Should().HaveCount(3);
        files.Select(f => f.FullPath).Should().Contain(@"C:\data\activity.fit");
        files.Select(f => f.FullPath).Should().Contain(@"C:\data\sub1\morning.fit");
        files.Select(f => f.FullPath).Should().Contain(@"C:\data\sub1\sub2\deep.fit");
    }

    [Fact]
    public async Task Single_file_path_directory_returns_only_files_in_that_directory()
    {
        _fileSystem.AddFitFile(@"C:\data\activity.fit");

        var files = await _discoveryService.DiscoverAsync(@"C:\data", CancellationToken.None);

        files.Should().HaveCount(1);
        files[0].FileName.Should().Be("activity.fit");
    }
}
