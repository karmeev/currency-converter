using System.Threading.RateLimiting;

namespace Currency.Api.Settings;

public record RateLimiterSettings(
    int PermitLimit,
    int DurationMilliseconds,
    int RejectionStatusCode,
    int QueueLimit,
    QueueProcessingOrder QueueOrder = QueueProcessingOrder.OldestFirst);