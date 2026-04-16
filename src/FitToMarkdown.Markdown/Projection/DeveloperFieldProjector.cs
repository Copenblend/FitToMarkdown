using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.Summaries;

namespace FitToMarkdown.Markdown.Projection;

internal static class DeveloperFieldProjector
{
    private const int MaxRecordSamples = 10;

    internal static IReadOnlyList<DeveloperFieldGroup> Project(FitFileDocument document)
    {
        var groups = new List<DeveloperFieldGroup>();

        var activity = document.ActivityContent;
        if (activity is not null)
        {
            foreach (var session in activity.Sessions)
            {
                AddGroupIfNonEmpty(groups, FitDeveloperFieldOwnerType.Session,
                    session.DeveloperFields, session.Range.StartTimeUtc,
                    sessionIndex: session.SessionIndex);

                foreach (var lap in session.Laps)
                {
                    AddGroupIfNonEmpty(groups, FitDeveloperFieldOwnerType.Lap,
                        lap.DeveloperFields, lap.Range.StartTimeUtc,
                        sessionIndex: session.SessionIndex, lapIndex: lap.LapIndex);
                }

                foreach (var length in session.Lengths)
                {
                    AddGroupIfNonEmpty(groups, FitDeveloperFieldOwnerType.Length,
                        length.DeveloperFields, length.Range.StartTimeUtc,
                        sessionIndex: session.SessionIndex, lengthIndex: length.LengthIndex);
                }

                int recordsSampled = 0;
                foreach (var record in session.Records)
                {
                    if (recordsSampled >= MaxRecordSamples)
                        break;

                    if (record.DeveloperFields.Count > 0)
                    {
                        AddGroupIfNonEmpty(groups, FitDeveloperFieldOwnerType.Record,
                            record.DeveloperFields, record.TimestampUtc,
                            sessionIndex: session.SessionIndex, recordIndex: record.RecordIndex);
                        recordsSampled++;
                    }
                }
            }

            foreach (var evt in activity.Events)
            {
                AddGroupIfNonEmpty(groups, FitDeveloperFieldOwnerType.Event,
                    evt.DeveloperFields, evt.TimestampUtc,
                    eventIndex: evt.EventIndex);
            }
        }

        if (document.Workout is not null)
        {
            foreach (var step in document.Workout.Steps)
            {
                AddGroupIfNonEmpty(groups, FitDeveloperFieldOwnerType.WorkoutStep,
                    step.DeveloperFields, null);
            }
        }

        // Assign sequential order
        for (int i = 0; i < groups.Count; i++)
        {
            groups[i] = groups[i] with { Order = i + 1 };
        }

        return groups;
    }

    private static void AddGroupIfNonEmpty(
        List<DeveloperFieldGroup> groups,
        FitDeveloperFieldOwnerType ownerType,
        IReadOnlyList<FitDeveloperFieldValue> fields,
        DateTimeOffset? timestamp,
        int? sessionIndex = null,
        int? lapIndex = null,
        int? lengthIndex = null,
        int? recordIndex = null,
        int? eventIndex = null)
    {
        if (fields.Count == 0)
            return;

        groups.Add(new DeveloperFieldGroup
        {
            OwnerType = ownerType,
            SessionIndex = sessionIndex,
            LapIndex = lapIndex,
            LengthIndex = lengthIndex,
            RecordIndex = recordIndex,
            EventIndex = eventIndex,
            TimestampUtc = timestamp,
            Values = fields,
        });
    }
}
