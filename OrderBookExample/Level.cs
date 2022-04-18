namespace OrderBookExample
{
    public struct Level
    {
        // Цена на данной позиции.
        public decimal Price;

        // Объем на данной позиции.
        public decimal Size;

        // Кумулятивный объем на данной позиции.
        public decimal? CumulSize;

        public Level(decimal price, decimal size) : this()
        {
            Price = price;
            Size = size;
        }

        public Level(decimal price, decimal size, decimal cumulSize) : this(price, size)
        {
            CumulSize = cumulSize;
        }

        public override string ToString()
        {
            return $"{Price}/{Size}/{CumulSize}";
        }
    }
}
