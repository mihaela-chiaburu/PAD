using Common.Models;
using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using SyncNode.Settings;
using Common.Utilities;

namespace SyncNode.Services
{
    public class SyncWorkJobService:IHostedService
    {
        private readonly ConcurrentDictionary<Guid, SyncEntity> documents = 
            new ConcurrentDictionary<Guid, SyncEntity>();
        private readonly IMovieAPISettings _settings;

        private Timer _timer;

        public SyncWorkJobService(IMovieAPISettings settings)
        {
            _settings = settings;
        }

        public void AddItem(SyncEntity item)
        {
            SyncEntity document = null;
            bool exists = documents.TryGetValue(item.Id, out document);

            if (!exists || (exists && (item.LastChangedAt > document.LastChangedAt)))
            {
                documents[item.Id] = item;
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoSendWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(20));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void DoSendWork(object state)
        {
            foreach (var document in documents)
            {
                SyncEntity entity = null;
                var isPresent = documents.TryRemove(document.Key, out entity);

                if (isPresent)
                {
                    var receivers = _settings.Hosts.Where(x => !x.Contains(entity.Origin));

                    foreach (var receiver in receivers)
                    {
                        var url = $"{receiver}/{entity.ObjectType}/sync";

                        try
                        {
                            var result = HttpClientUtility.SendJson(entity.JsonData, url, entity.SyncType);
                            if (!result.IsSuccessStatusCode)
                            {
                                // log error
                            }
                        }
                        catch (Exception ex)
                        {
                            // log exception
                        }
                    }
                }
            }
        }
    }
}
