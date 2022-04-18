using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderBookExample
{
    public enum Side
    {
        Bid, //наверху максимальная цена, по которой покупатель согласен купить товар, выгодно - наименьшую (первую)
        Ask //наверху наименьшая цена, по которой продавец согласен продать товар, выгодно - наибольшую (последнюю)
    }
    
    public class OrderBookBase
    {
        protected decimal priceMultiplier;
        protected decimal sizeMultiplier;

        protected OrderBookBase() {}
        
        // pricePrecision -- сколько цифр после запятой в ценах
        // sizePrecision -- сколько цифр после запятой в объемах
        public OrderBookBase(uint pricePrecision, uint sizePrecision)
        {
            priceMultiplier = (decimal)Math.Pow(10, -pricePrecision);
            sizeMultiplier = (decimal)Math.Pow(10, -sizePrecision);
        }
    }

    public class OrderBook: OrderBookBase, IOrderBook
    {
        public SortedDictionary<decimal, decimal> Bids; 
        public SortedDictionary<decimal, decimal> Asks;
        public SortedDictionary<decimal, decimal>? ChosenSide;
        public IEnumerator<KeyValuePair<decimal, decimal>>? Enumerator;

        public OrderBook()
        {
            Bids = new SortedDictionary<decimal, decimal>();
            Asks = new SortedDictionary<decimal, decimal>();
        }
        public void SetChosenSide(Side side)
        {
            ChosenSide = side == Side.Ask ? Asks : Bids;
        }
        
        public void SetSideEnumenator(Side side)
        {
            Enumerator = side == Side.Bid ? Bids.GetEnumerator() : Asks.Reverse().GetEnumerator();
            Enumerator.MoveNext();
        }
        
        public void Update(Side side, decimal price, decimal size, bool ignoreError = false)
        {
            SetChosenSide(side);
            if (ChosenSide != null) ChosenSide[price] = size;
        }

        public void Fill(Side side, IEnumerable<Tuple<decimal, decimal>> data)
        {
            SetChosenSide(side);

            foreach (var tuple in data)
            {
                if (ChosenSide != null && !ChosenSide.ContainsKey(tuple.Item1))
                    ChosenSide.Add(tuple.Item1, tuple.Item2);
                else
                    Update(side, tuple.Item1, tuple.Item2, false);
            }
        }

        public Tuple<int, int> Clear()
        {
            var res = new Tuple<int, int>(Asks.Count, Bids.Count);
            Asks.Clear();
            Bids.Clear();
            return res;
        }

        public BidAsk GetBidAsk()
        {
            var bidPair = Bids.First(); //выгодно - наименьшую (первую)
            var askPair = Asks.Last(); //выгодно - наибольшую (последнюю)
            
            var bidAsk = new BidAsk()
            {
                AskPrice = askPair.Key,
                AskVolume = askPair.Value, 
                BidPrice = bidPair.Key,
                BidVolume = bidPair.Value
            };
            return bidAsk;
        }

       

        public Level[] GetTop(Side side, int count, bool cumulative = false)
        {
            List<Level> level = new List<Level>();
            decimal previousSize = 0;
            SetSideEnumenator(side);

            for (int i = 0; i < count; i++)
            {
                if (cumulative)
                {
                    if (Enumerator != null)
                    {
                        level.Add(new Level(Enumerator.Current.Key, Enumerator.Current.Value,
                            Enumerator.Current.Value + previousSize));
                        previousSize = Enumerator.Current.Value;
                    }
                }
                else if (Enumerator != null) level.Add(new Level(Enumerator.Current.Key, Enumerator.Current.Value));

                Enumerator?.MoveNext();
            }
            return level.ToArray();
        }

        public Level[] GetTop(Side side, decimal price, bool cumulative = false)
        {
            List<Level> level = new List<Level>();
            decimal previousSize = 0;
            SetSideEnumenator(side);

            while(Enumerator != null && Enumerator.Current.Key != price)
            {
                if (cumulative)
                {
                    level.Add(new Level(Enumerator.Current.Key, Enumerator.Current.Value, Enumerator.Current.Value + previousSize));
                    previousSize = Enumerator.Current.Value;
                }
                else
                    level.Add(new Level(Enumerator.Current.Key, Enumerator.Current.Value));
                Enumerator.MoveNext();
            }
            return level.ToArray();
        }

        public decimal? GetPriceWhenCumulGreater(Side side, decimal cumul)
        {
            decimal previousSize = 0;
            SetChosenSide(side);
            SetSideEnumenator(side);
            while (Enumerator != null && Enumerator.MoveNext())
            {
                var currentCumul = Enumerator.Current.Value + previousSize;
                if (currentCumul == cumul)
                    return Enumerator.Current.Key;
            }

            return 0;
        }

        public bool IsEmpty()
        {
            return Bids.Count == 0 && Asks.Count == 0;
        }
    }
}
