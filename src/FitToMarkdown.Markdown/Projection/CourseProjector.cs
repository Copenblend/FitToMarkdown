using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Summaries;

namespace FitToMarkdown.Markdown.Projection;

internal static class CourseProjector
{
    internal static IReadOnlyList<CoursePointRow> Project(FitFileDocument document)
    {
        var course = document.Course;
        if (course is null || course.Points.Count == 0)
            return [];

        var rows = new List<CoursePointRow>(course.Points.Count);
        foreach (var point in course.Points)
        {
            rows.Add(new CoursePointRow
            {
                Order = point.PointIndex + 1,
                PointName = point.PointName,
                PointType = point.PointType,
                DistanceMeters = point.DistanceMeters,
                Position = point.Position,
            });
        }

        return rows;
    }
}
