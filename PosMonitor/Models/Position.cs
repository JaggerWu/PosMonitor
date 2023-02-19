namespace PosMonitor.Models
{
    /// <summary>
    /// 
    /// </summary>
    public record Position
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int PositionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Ticker { get; set; } = String.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double SpotPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int QtyCurrent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int QtyStart { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int QtyChange => QtyCurrent - QtyStart;
       
        /// <summary>
        /// 
        /// </summary>       
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double LastPriceChange { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int LastQtyChange { get; set; }
    }
}
