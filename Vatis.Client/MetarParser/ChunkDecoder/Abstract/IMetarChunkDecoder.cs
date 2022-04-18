using System.Collections.Generic;

namespace MetarDecoder
{
    public interface IMetarChunkDecoder
    {
        /// <summary>
        /// Get the regular expression that will be used by the chunk decoder
        /// Each chunk decoder must delcare its own.
        /// </summary>
        /// <returns></returns>
        string GetRegex();
        /// <summary>
        /// Decode the chunk passed to the decoder and return the decoded information object
        /// </summary>
        /// <param name="remainingMetar"></param>
        /// <param name="withCavok"></param>
        /// <returns></returns>
        Dictionary<string, object> Parse(string remainingMetar, bool withCavok = false);
    }
}
