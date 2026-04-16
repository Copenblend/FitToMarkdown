using System.Globalization;
using System.Text;
using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Markdown.Formatting;

namespace FitToMarkdown.Markdown.Rendering.Sections;

internal static class UserProfileRenderer
{
    internal static void Render(FitUserProfile userProfile, StringBuilder sb)
    {
        var writer = new MarkdownTextWriter();
        writer.AppendHeading(2, "User Profile");

        AppendIfPresent(writer, "Friendly Name", MarkdownEscaper.SanitizeFitString(userProfile.FriendlyName));
        AppendIfPresent(writer, "Gender", userProfile.Gender?.ToString());

        if (userProfile.AgeYears is not null)
            writer.AppendBulletItem($"**Age:** {userProfile.AgeYears.Value.ToString(CultureInfo.InvariantCulture)} years");

        if (userProfile.HeightCentimeters is not null)
            writer.AppendBulletItem($"**Height:** {userProfile.HeightCentimeters.Value.ToString("F1", CultureInfo.InvariantCulture)} cm");

        if (userProfile.WeightKilograms is not null)
            writer.AppendBulletItem($"**Weight:** {userProfile.WeightKilograms.Value.ToString("F1", CultureInfo.InvariantCulture)} kg");

        if (userProfile.RestingHeartRateBpm is not null)
            writer.AppendBulletItem($"**Resting HR:** {MarkdownValueFormatter.FormatHeartRate(userProfile.RestingHeartRateBpm.Value)} bpm");

        if (userProfile.DefaultMaxHeartRateBpm is not null)
            writer.AppendBulletItem($"**Max HR:** {MarkdownValueFormatter.FormatHeartRate(userProfile.DefaultMaxHeartRateBpm.Value)} bpm");

        if (userProfile.DefaultMaxRunningHeartRateBpm is not null)
            writer.AppendBulletItem($"**Max Running HR:** {MarkdownValueFormatter.FormatHeartRate(userProfile.DefaultMaxRunningHeartRateBpm.Value)} bpm");

        if (userProfile.DefaultMaxBikingHeartRateBpm is not null)
            writer.AppendBulletItem($"**Max Biking HR:** {MarkdownValueFormatter.FormatHeartRate(userProfile.DefaultMaxBikingHeartRateBpm.Value)} bpm");

        AppendIfPresent(writer, "Language", userProfile.Language);
        AppendIfPresent(writer, "Activity Class", userProfile.ActivityClass?.ToString(CultureInfo.InvariantCulture));

        sb.Append(writer.ToString());
    }

    private static void AppendIfPresent(MarkdownTextWriter writer, string label, string? value)
    {
        if (!string.IsNullOrEmpty(value))
            writer.AppendBulletItem($"**{label}:** {value}");
    }
}
