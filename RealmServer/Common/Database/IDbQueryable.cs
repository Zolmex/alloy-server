using System.Collections.Generic;

namespace Common.Database;

public interface IDbQueryable {
    static abstract IEnumerable<string> GetIncludes();
}