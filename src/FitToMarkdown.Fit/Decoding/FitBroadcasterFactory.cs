using Dynastream.Fit;
using FitToMarkdown.Core.Parsing;

namespace FitToMarkdown.Fit.Decoding;

/// <summary>
/// Factory for creating and wiring a <see cref="MesgBroadcaster"/> to a <see cref="Decode"/> instance.
/// </summary>
internal static class FitBroadcasterFactory
{
    /// <summary>
    /// Creates a <see cref="MesgBroadcaster"/> wired to the given decoder.
    /// </summary>
    /// <param name="decoder">The FIT decoder instance.</param>
    /// <param name="options">Parse options influencing broadcaster configuration.</param>
    /// <returns>The wired broadcaster and whether post-broadcast processing is needed.</returns>
    public static (MesgBroadcaster Broadcaster, bool NeedsPostBroadcast) Create(Decode decoder, FitParseOptions options)
    {
        var broadcaster = new MesgBroadcaster();

        // Connect decoder events to broadcaster
        decoder.MesgEvent += broadcaster.OnMesg;
        decoder.MesgDefinitionEvent += broadcaster.OnMesgDefinition;

        // BufferedMesgBroadcaster does not exist in SDK v1.0.1 — always use plain MesgBroadcaster
        return (broadcaster, false);
    }
}
