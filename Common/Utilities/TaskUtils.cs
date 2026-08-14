using System;
using System.Threading;

namespace Common.Utilities;

public static class TaskUtils {
    public static CancellationToken Timeout(int seconds) {
        return new CancellationTokenSource(seconds * 1000).Token;
    }
}