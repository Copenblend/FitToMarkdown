using FitToMarkdown.Cli.Rendering;
using FitToMarkdown.Core.Models;
using FluentAssertions;
using Spectre.Console.Testing;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Rendering;

public sealed class InfoTableRendererTests
{
    private readonly TestConsole _console;
    private readonly InfoTableRenderer _renderer;

    public InfoTableRendererTests()
    {
        _console = new TestConsole();
        _console.Profile.Width = 120;
        _renderer = new InfoTableRenderer(_console);
    }

    [Fact]
    public void RenderSummary_renders_all_populated_fields()
    {
        var summary = new FitFileInfoSummary
        {
            Sport = "Running",
            ManufacturerName = "Garmin",
            ProductName = "Forerunner 265",
            ProductId = 4257,
            TotalDistanceMeters = 8500.0,
            LapCount = 5,
        };

        var renderable = _renderer.RenderSummary(summary);
        _console.Write(renderable);

        _console.Output.Should().Contain("Running");
        _console.Output.Should().Contain("Garmin");
        _console.Output.Should().Contain("Forerunner 265");
        _console.Output.Should().Contain("8.50 km");
    }

    [Fact]
    public void RenderSummary_renders_na_for_missing_fields()
    {
        var summary = new FitFileInfoSummary();

        var renderable = _renderer.RenderSummary(summary);
        _console.Write(renderable);

        _console.Output.Should().Contain("n/a");
    }

    [Fact]
    public void RenderDevices_renders_device_table()
    {
        var devices = new List<FitDeviceSummary>
        {
            new()
            {
                DeviceType = "Watch",
                ManufacturerName = "Garmin",
                ProductName = "Forerunner 265",
                ProductId = 4257,
                SerialNumber = 3456789012,
                BatteryStatus = "Good",
                BatteryVoltage = 3.95,
                Descriptor = "Primary",
            },
        };

        var renderable = _renderer.RenderDevices(devices);
        renderable.Should().NotBeNull();

        _console.Write(renderable!);

        _console.Output.Should().Contain("Watch");
        _console.Output.Should().Contain("Primary");
    }

    [Fact]
    public void RenderDevices_returns_null_for_empty_device_list()
    {
        var renderable = _renderer.RenderDevices([]);

        renderable.Should().BeNull();
    }
}
