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

        private async void DoSendWork(object state)
        {
            foreach (var kvp in documents)
            {
                if (documents.TryRemove(kvp.Key, out var entity))
                {
                    var receivers = _settings.Hosts.Where(x => !x.Contains(entity.Origin));

                    foreach (var receiver in receivers)
                    {
                        var url = $"{receiver}/{entity.ObjectType}/sync";
                        try
                        {
                            using var client = new HttpClient();
                            HttpResponseMessage response;

                            if (entity.SyncType == "PUT")
                                response = await client.PutAsJsonAsync(url, entity.JsonData);
                            else if (entity.SyncType == "POST")
                                response = await client.PostAsJsonAsync(url, entity.JsonData);
                            else if (entity.SyncType == "DELETE")
                                response = await client.DeleteAsync(url);
                            else
                                continue;

                            if (!response.IsSuccessStatusCode)
                            {
                                Console.WriteLine($"Failed to sync {entity.Id} to {receiver}: {response.StatusCode}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Exception syncing {entity.Id} to {receiver}: {ex.Message}");
                        }
                    }
                }
            }
        }

    }
}
