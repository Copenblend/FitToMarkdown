using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Summaries;

namespace FitToMarkdown.Markdown.Projection;

internal static class WorkoutProjector
{
    internal static IReadOnlyList<WorkoutStepRow> Project(FitFileDocument document)
    {
        var workout = document.Workout;
        if (workout is null || workout.Steps.Count == 0)
            return [];

        var rows = new List<WorkoutStepRow>(workout.Steps.Count);
        foreach (var step in workout.Steps)
        {
            rows.Add(new WorkoutStepRow
            {
                Order = step.StepIndex + 1,
                StepName = step.StepName,
                DurationType = step.DurationType,
                DurationValue = step.DurationValue,
                TargetType = step.TargetType,
                TargetRange = step.TargetRange,
                Intensity = step.Intensity,
                Notes = step.Notes,
                Equipment = step.Equipment,
            });
        }

        return rows;
    }
}
