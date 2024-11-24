using System;

namespace AutoBuyer.Logic.Domain
{
    public class StockCommand : ValueObject<StockCommand>
    {
        private readonly string _content;

        public StockCommand(string content)
        {
            _content = content;
        }

        public static StockCommand Buy(int price, int number)
        {
            return new StockCommand(string.Format("Command: BUY; Price: {0}; Number: {1}", price, number));
        }

        public static StockCommand None()
        {
            return new StockCommand(string.Empty);
        }

        public override string ToString()
        {
            return _content;
        }

        public static implicit operator string(StockCommand s) => s.ToString();

        protected override bool Equals(StockCommand other)
        {
            return this.GetHashCode().Equals(other.GetHashCode());
        }

        protected override int GetHashCode(ValueObject<StockCommand> @object)
        {
            if (@object is StockCommand command)
            {
                return command._content.GetHashCode();
            }
            throw new ArgumentException("object should be of type StockCommand");
        }
    }
}
