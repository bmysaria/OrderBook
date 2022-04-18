namespace OrderBookExample
{
    public class BidAsk
    {
        public decimal AskPrice { get; set; }
        public decimal AskVolume { get; set; }
        public decimal BidPrice { get; set; }
        public decimal BidVolume { get; set; }

        public decimal Price => (BidPrice + AskPrice) / 2;

        public override string ToString()
        {
            return $"Bids-min:{BidPrice}/{BidVolume}-Asks-max: {AskPrice}/{AskVolume}";
        }
    }
}
