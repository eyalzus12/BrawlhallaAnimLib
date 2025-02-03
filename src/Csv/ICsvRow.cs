using System.Collections.Generic;

namespace BrawlhallaAnimLib.Csv;

public interface ICsvRow
{
    IEnumerable<KeyValuePair<string, string>> ColEntries { get; }
}