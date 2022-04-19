using System;
using System.Collections.Generic;
using System.Linq;
using OrderBookExample;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var orderBook = new OrderBook();
            var askTuple =  new List<Tuple<decimal, decimal>>();
            askTuple.Add(new Tuple<decimal, decimal>(125.95m, 14));
            askTuple.Add(new Tuple<decimal, decimal>(125.96m, 24));
            askTuple.Add(new Tuple<decimal, decimal>(125.97m, 2));
            askTuple.Add(new Tuple<decimal, decimal>(125.98m, 1));
            askTuple.Add(new Tuple<decimal, decimal>(125.99m, 7));
            askTuple.Add(new Tuple<decimal, decimal>(126.00m, 6));
            askTuple.Add(new Tuple<decimal, decimal>(126.01m, 76));
            askTuple.Add(new Tuple<decimal, decimal>(126.02m, 115));
            askTuple.Add(new Tuple<decimal, decimal>(126.03m, 10));
            askTuple.Add(new Tuple<decimal, decimal>(126.04m, 4));
            orderBook.Fill(Side.Ask, askTuple);
           //Console.WriteLine("Asks:");
           //orderBook.PrintSide(Side.Ask);
           //Console.WriteLine("Get top:");
           
           
           var bidTuple = new List<Tuple<decimal, decimal>>();
            bidTuple.Add(new Tuple<decimal, decimal>(125.85m, 7));
            bidTuple.Add(new Tuple<decimal, decimal>(125.84m, 11));
            bidTuple.Add(new Tuple<decimal, decimal>(125.83m, 65));
            bidTuple.Add(new Tuple<decimal, decimal>(125.82m, 95));
            bidTuple.Add(new Tuple<decimal, decimal>(125.81m, 125));
            bidTuple.Add(new Tuple<decimal, decimal>(125.80m, 10));
            bidTuple.Add(new Tuple<decimal, decimal>(125.79m, 98));
            bidTuple.Add(new Tuple<decimal, decimal>(125.78m, 119));
            bidTuple.Add(new Tuple<decimal, decimal>(125.77m, 26));
            bidTuple.Add(new Tuple<decimal, decimal>(125.76m, 287));
            orderBook.Fill(Side.Bid, bidTuple);
           // Console.WriteLine("Bids:");
            //orderBook.PrintSide(Side.Bid);
            /*var res = orderBook.GetTop(Side.Bid, 125.80m, true);
            foreach (var elem in res)
            {
                Console.WriteLine(elem.ToString());
            }*/
            Console.WriteLine(orderBook.GetPriceWhenCumulGreater(Side.Ask, 130));
        }
    }
}