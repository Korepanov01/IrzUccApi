using IrzUccApi.Db;
using IrzUccApi.Models.Configurations;

namespace IrzUccApi.Services
{
    public class MemoryСleaner : BackgroundService
    {
        private readonly MemoryCleanerConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MemoryСleaner(
            MemoryCleanerConfiguration configuration,
            IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var delayPeriod = new TimeSpan(_configuration.DeletionPeriodDays, 0, 0, 0);
            var messagesDeletionDate = DateTime.UtcNow.AddDays(-_configuration.MessageLifetimeDays);
            var eventsDeletionDate = DateTime.UtcNow.AddDays(-_configuration.EventsLifetimeDays);
            
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(delayPeriod, cancellationToken);

                    using var scope = _serviceScopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
                    dbContext!.RemoveRange(dbContext.Messages.Where(m => m.DateTime < messagesDeletionDate));
                    dbContext.RemoveRange(dbContext.Events.Where(e => e.End < eventsDeletionDate));
                    await dbContext.SaveChangesAsync();
                }
                catch (TaskCanceledException) { }
            }
        }
    }
}