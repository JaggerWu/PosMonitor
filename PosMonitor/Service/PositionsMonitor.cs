using PosMonitor.Models;
using System.Collections.Concurrent;
using System.Reactive.Subjects;


namespace PosMonitor.Service
{
    /// <summary>
    /// Monitoring the price and position quantit. update every one second
    /// </summary>
    public class PositionsMonitor: BackgroundService
    {		
        private ConcurrentDictionary<int, Position> _cache;		
        private readonly PeriodicTimer _periodicTimer;		
        private readonly ILogger<PositionsMonitor> _logger;

        private const double _priceRangePercent = 0.002;
        private const double _qtyRangePercent = 0.001;

        private long _seq;
        private BehaviorSubject<long> _currentSquenceNumber;
        public IObservable<long> CurrentSquenceNumber => _currentSquenceNumber;

        private readonly Random _rand;		

        public PositionsMonitor(ILogger<PositionsMonitor> logger)
        {			
            _logger = logger;
            _periodicTimer = new PeriodicTimer(new TimeSpan(0, 0, 1));			
            _cache = new ConcurrentDictionary<int, Position>();

            _rand = new Random();
            _seq = 0;
            _currentSquenceNumber = new BehaviorSubject<long>(0);

            // add sample data
            DateTime t = DateTime.Now;			
            _cache.TryAdd(1, new Position() { PositionId = 1, Ticker = "700.HK", SpotPrice = 373.800, QtyStart = 10000, QtyCurrent = 15000, UpdateTime = t });
            _cache.TryAdd(2, new Position() { PositionId = 2, Ticker = "9988.HK", SpotPrice = 99.450, QtyStart = 10000, QtyCurrent = 15000, UpdateTime = t });
            _cache.TryAdd(3, new Position() { PositionId = 3, Ticker = "3690.HK", SpotPrice = 144.200, QtyStart = 10000, QtyCurrent = 15000, UpdateTime = t });
            _cache.TryAdd(4, new Position() { PositionId = 4, Ticker = "2269.HK", SpotPrice = 56.650, QtyStart = 10000, QtyCurrent = 15000, UpdateTime = t });
            _cache.TryAdd(5, new Position() { PositionId = 5, Ticker = "9618.HK", SpotPrice = 210.6, QtyStart = 10000, QtyCurrent = 15000, UpdateTime = t });
            _cache.TryAdd(6, new Position() { PositionId = 6, Ticker = "9888.HK", SpotPrice = 141.3, QtyStart = 10000, QtyCurrent = 15000, UpdateTime = t });
            _cache.TryAdd(7, new Position() { PositionId = 7, Ticker = "1211.HK", SpotPrice = 229.6, QtyStart = 10000, QtyCurrent = 15000, UpdateTime = t });
            _cache.TryAdd(8, new Position() { PositionId = 8, Ticker = "1299.HK", SpotPrice = 83.65, QtyStart = 10000, QtyCurrent = 15000, UpdateTime = t });
            _cache.TryAdd(9, new Position() { PositionId = 9, Ticker = "1725.HK", SpotPrice = 11.24, QtyStart = 10000, QtyCurrent = 15000, UpdateTime = t });
            _cache.TryAdd(10, new Position() { PositionId = 10, Ticker = "1024.HK", SpotPrice = 59.95, QtyStart = 10000, QtyCurrent = 15000, UpdateTime = t });
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        public async Task<Position[]> GetNetPositions()
        {
            return await Task.FromResult(_cache.Values.ToArray());

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _currentSquenceNumber.OnNext(TryUpdateAllPositions());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception in {MethodName}", nameof(ExecuteAsync));
                }
                await _periodicTimer.WaitForNextTickAsync(stoppingToken);
            }
        }

        private long TryUpdateAllPositions()
        {
            DateTime updateTime = DateTime.Now;
            Parallel.ForEach(_cache.Values, p => UpdatePriceAndQty(p, updateTime));
            _logger.LogInformation($"Positions snapshot updated. SequenceNumber: {++_seq}");

            return _seq;
        }

        private void UpdatePriceAndQty(Position position, DateTime updateTime)
        {
            // Randomly choose whether to udpate this stock or not
            var r = _rand.NextDouble();
            if (r > 0.5)
                return;

            // Update the stock price by a random factor of the range percent            
            var percentChange = (_rand.Next() % 10 - 5) * _priceRangePercent;            
            var priceChange = position.SpotPrice * percentChange;          

            position.SpotPrice += priceChange;
            position.LastPriceChange = priceChange;

            // Update the stock quantity by a random factor of the range percent  
            var qtyPercentChange = (_rand.Next() % 10 - 5) * _qtyRangePercent;			
            int qtyChange = (int)(position.QtyCurrent * qtyPercentChange);            

            position.QtyCurrent += qtyChange;
            position.LastQtyChange = qtyChange;

            position.UpdateTime = updateTime;            
        }

    }
}

