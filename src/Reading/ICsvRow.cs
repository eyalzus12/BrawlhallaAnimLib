using System.Collections.Generic;

namespace BrawlhallaAnimLib.Reading;

public interface ICsvRow
{
    IEnumerable<KeyValuePair<string, string>> ColEntries { get; }
}