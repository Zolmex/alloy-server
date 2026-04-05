using System.Collections.Generic;
using System.Linq;

namespace Common.Database;

public interface IDbQueryable
{
    static abstract IEnumerable<string> GetIncludes();
}