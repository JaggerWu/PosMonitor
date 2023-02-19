using Microsoft.AspNetCore.SignalR;
using PosMonitor.Models;


namespace PosMonitor.Service
{
    /// <summary>
    /// Hub Service manages all connections and sends the real-time to Clients
    /// </summary>   
    public class PositionHub: Hub
    {
        private readonly PositionsMonitor _monitor;
        private readonly ILogger<PositionHub> _logger;        
        private readonly object _locker = new();

        private static Dictionary<string, IDisposable?> _observers = new();        

        public PositionHub(ILogger<PositionHub> logger, PositionsMonitor monitor)
        {
            _logger = logger;
            _monitor = monitor;
        }

        public virtual async Task Send(Position[] array, IClientProxy client)
        {
            await client.SendAsync("updatepositions", array);
        }

        public async Task TrySend(Position[] array, string connectionId, IClientProxy client, long seqNumber)
        {
            try
            {
                await Send(array, client);
                _logger.LogInformation($"Data sent to Client {connectionId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in {MethodName}", nameof(Send));
            }
        }

        public override Task OnConnectedAsync()
        {
            var connection = Context.ConnectionId;
            var cliens = Clients;            

            lock (_locker)
            {
                if (!_observers.ContainsKey(connection))
                {
                    _logger.LogInformation($"Client {connection} establishes connection.");
                    _observers[connection] = _monitor.CurrentSquenceNumber.Subscribe(
                        async (long seq) =>
                        {
                            var client = cliens.Client(connection);
                            var data = await _monitor.GetPositionsAsync();

                            await TrySend(data, connection, client, seq);
                        });                    
                }
                else
                {
                    _observers[connection]?.Dispose();
                    _logger.LogInformation($"Client {connection} refreshes its connection.");
                    _observers[connection] = _monitor.CurrentSquenceNumber.Subscribe(
                        async (long seq) =>
                        {
                            var client = cliens.Client(connection);
                            var data = await _monitor.GetPositionsAsync();

                            await TrySend(data, connection, client, seq);
                        });                    
                }
            }

            return base.OnConnectedAsync();
        }

        private void StopObserver(string connectionId)
        {
            lock (_locker)
            {
                _observers.TryGetValue(connectionId, out var observer);

                observer?.Dispose();

                _observers.Remove(connectionId);
            }
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var connection = Context.ConnectionId;            
            _logger.LogInformation($"Client {connection} disconnected");

            StopObserver(connection);

            return base.OnDisconnectedAsync(exception);
        }
    }
}
