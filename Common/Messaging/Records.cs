namespace Common.Messaging;

public readonly record struct GameServerStatus(
    int Players,
    float Stability, // Score from 1.0 to 5.0
    long UptimeMs
);