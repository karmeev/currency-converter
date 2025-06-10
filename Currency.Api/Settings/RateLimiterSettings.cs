using System.Threading.RateLimiting;

namespace Currency.Api.Settings;

public record RateLimiterSettings(
    int PermitLimit,
    int DurationMilliseconds,
    int QueueLimit,
    QueueProcessingOrder QueueOrder = QueueProcessingOrder.OldestFirst);