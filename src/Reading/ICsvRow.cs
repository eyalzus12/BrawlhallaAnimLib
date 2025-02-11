using System.Collections.Generic;

namespace BrawlhallaAnimLib.Reading;

public interface ICsvRow
{
    string RowKey { get; }
    IEnumerable<KeyValuePair<string, string>> ColEntries { get; }
}