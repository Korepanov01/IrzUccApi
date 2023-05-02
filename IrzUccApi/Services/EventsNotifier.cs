using IrzUccApi.Db;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi.Services
{
    public class EventsNotifier: BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EventsNotifier( IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var delayPeriod = new TimeSpan(1, 0, 0, 0);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(delayPeriod, cancellationToken);

                    using var scope = _serviceScopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetService<AppDbContext>();

                    var from = DateTime.UtcNow;
                    var to = from.AddDays(1);

                    var events = await dbContext!.Events
                    .Where(e => e.Start > from && e.Start < to)
                    .ToDictionaryAsync(e => e.Title, e => e.Listeners.Select(u => u.Email).Append(e.Creator.Email).ToArray());

                    var emailService = scope.ServiceProvider.GetService<EmailService>();
                    foreach (var e in events)
                        foreach (var email in e.Value)
                            await emailService!.SendEventNotification(email, e.Key);
                }
                catch (TaskCanceledException) { }
            }
        }
    }
}
