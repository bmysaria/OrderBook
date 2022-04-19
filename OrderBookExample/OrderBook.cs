using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OrderBookExample
{
    public enum Side
    {
        Bid, //продажа, наверху (первый элемент) максимальная цена - reverse
        Ask //покупка, наверху (первый элемент) наименьшая цена
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

    class DescendingComparer<T> : IComparer<T> where T : IComparable<T> {
        public int Compare(T x, T y) {
            return y.CompareTo(x);
        }
    }

    public class OrderBook: OrderBookBase, IOrderBook
    {
        private SortedDictionary<decimal, decimal> Bids; 
        private SortedDictionary<decimal, decimal> Asks;

        public OrderBook()
        {
            Bids = new SortedDictionary<decimal, decimal>(new DescendingComparer<decimal>());
            Asks = new SortedDictionary<decimal, decimal>();
        }
        public SortedDictionary<decimal,decimal> GetChosenSide(Side side)
        {
            return  side == Side.Ask ? Asks : Bids;
        }

        public void Update(Side side, decimal price, decimal size, bool ignoreError = false)
        {
            var chosenSide = GetChosenSide(side);
            chosenSide[price] = size;
        }

        public void Fill(Side side, IEnumerable<Tuple<decimal, decimal>> data)
        {
            var chosenSide = GetChosenSide(side);

            foreach (var tuple in data)
            {
                if (chosenSide != null && !chosenSide.ContainsKey(tuple.Item1))
                    chosenSide.Add(tuple.Item1, tuple.Item2);
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
            var bidAsk = new BidAsk()
            {
                AskPrice = Asks.First().Key,
                AskVolume = Asks.First().Value, 
                BidPrice = Bids.First().Key,
                BidVolume = Bids.First().Value
            };
            return bidAsk;
        }
        
        public Level[] GetTop(Side side, int count, bool cumulative = false)
        {
            var chosenSide = GetChosenSide(side);
            var res = new Level[Math.Min(count, chosenSide.Count())];
            decimal previousSize = 0;
            using var en = chosenSide.GetEnumerator();
            
            for (int i = 0; i < count; i++)
            {
                en.MoveNext();
                if (!cumulative)
                    res[i] = new Level(en.Current.Key, en.Current.Value);
                else
                {
                    res[i] = new Level(en.Current.Key, en.Current.Value, en.Current.Value + previousSize);
                    previousSize += en.Current.Value;
                }
            }
            return res;
        }

        public Level[] GetTop(Side side, decimal price, bool cumulative = false)
        {
            var chosenSide = GetChosenSide(side);
            var res = new List<Level>();
            using var en = chosenSide.GetEnumerator();
            decimal previousSize = 0;

            while (en.MoveNext() && en.Current.Key != price)
            {
                if (!cumulative)
                    res.Add(new Level(en.Current.Key, en.Current.Value));
                {
                    res.Add(new Level(en.Current.Key, en.Current.Value, en.Current.Value + previousSize));
                    previousSize += en.Current.Value;
                }
            }
            return res.ToArray();
        }

        public decimal? GetPriceWhenCumulGreater(Side side, decimal cumul)
        {
            var chosenSide = GetChosenSide(side);
            using var en = chosenSide.GetEnumerator();
            decimal prevSize = 0;

            while (en.MoveNext())
            {
                var currentCumSize = en.Current.Value + prevSize;
                if (currentCumSize > cumul)
                    return en.Current.Key;
                prevSize += en.Current.Value;
            }
            return 0;
        }

        public bool IsEmpty()
        {
            return Bids.Count == 0 && Asks.Count == 0;
        }

        public void PrintSide(Side side)
        {
            var chosenSide = GetChosenSide(side);
            foreach (var elem in chosenSide)
            {
                Console.WriteLine(elem.ToString());
            }
        }
    }
}
