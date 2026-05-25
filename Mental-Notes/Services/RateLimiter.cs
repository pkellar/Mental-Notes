
using System.Collections.Concurrent;

namespace Services;

public class RateLimiter
{
    private readonly ConcurrentDictionary<string, SubmissionInfo> _ipAttempts = new();

    public bool IsLimited(string ip, int maxAttempts, TimeSpan window)
    {
        var now = DateTime.UtcNow;
        var info = _ipAttempts.GetOrAdd(ip, _ => new SubmissionInfo());

        lock (info.Lock)
        {
            // Remove attempts outside the window
            info.Attempts.RemoveAll(dt => dt < now - window);

            if (info.Attempts.Count >= maxAttempts)
                return true;

            info.Attempts.Add(now);
            return false;
        }
    }

    private class SubmissionInfo
    {
        public List<DateTime> Attempts { get; } = [];
        public object Lock { get; } = new();
    }
}
