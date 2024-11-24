using AutoBuyer.Logic.Domain;
using Should;
using Xunit;

namespace Tests
{

    public class StockCommandTests
    {
        [Fact]
        public void Buy_command_is_of_valid_content()
        {
            StockCommand command = StockCommand.Buy(123, 10);

            command.ToString().ShouldEqual("Command: BUY; Price: 123; Number: 10");
        }

        [Fact]
        public void Commands_are_equal_if_their_contents_match()
        {
            StockCommand cmd1 = StockCommand.Buy(123, 10);
            StockCommand cmd2 = StockCommand.Buy(123, 10);

            bool areEqual1 = cmd1.Equals(cmd2);
            bool areEqual2 = cmd1 == cmd2;
            bool areEqual3 = cmd1.GetHashCode() == cmd2.GetHashCode();
            bool areNotEqual = cmd1 != cmd2;

            areEqual1.ShouldBeTrue();
            areEqual2.ShouldBeTrue();
            areEqual3.ShouldBeTrue();
            areNotEqual.ShouldBeFalse();
        }
    }
}