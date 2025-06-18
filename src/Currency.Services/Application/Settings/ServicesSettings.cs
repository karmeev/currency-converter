namespace Currency.Services.Application.Settings;

public class ServicesSettings(WorkerSettings workers)
{
    public int RefreshTokenTtlInDays { get; init; }
    public WorkerSettings Worker { get; init; } = workers;
}