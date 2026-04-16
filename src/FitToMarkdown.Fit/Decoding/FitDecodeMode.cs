namespace FitToMarkdown.Fit.Decoding;

/// <summary>Selects decode listener coverage.</summary>
internal enum FitDecodeMode
{
    /// <summary>All message listeners are registered for full file parsing.</summary>
    FullParse,

    /// <summary>Only metadata-relevant listeners are registered for lightweight inspection.</summary>
    MetadataOnly,
}
