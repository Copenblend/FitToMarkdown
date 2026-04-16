using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Fit.Mapping;

/// <summary>
/// Creates <see cref="FitMessageIdentity"/> instances from decode-time sequencing data.
/// </summary>
internal static class FitMessageIdentityFactory
{
    /// <summary>
    /// Creates a <see cref="FitMessageIdentity"/> from parse-time properties.
    /// </summary>
    /// <param name="parseSequence">The zero-based decode order of the message.</param>
    /// <param name="messageIndex">The optional FIT message index field value.</param>
    /// <param name="isSynthetic">Whether the message was synthesized during recovery.</param>
    /// <param name="recoveredAfterFault">Whether the message was recovered after a decode fault.</param>
    /// <returns>A new <see cref="FitMessageIdentity"/> instance.</returns>
    public static FitMessageIdentity Create(int parseSequence, ushort? messageIndex = null, bool isSynthetic = false, bool recoveredAfterFault = false)
    {
        return new FitMessageIdentity
        {
            ParseSequence = parseSequence,
            MessageIndex = messageIndex,
            IsSynthetic = isSynthetic,
            RecoveredAfterDecodeFault = recoveredAfterFault,
        };
    }
}
