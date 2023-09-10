namespace LasticShopAdo.Interfaces
{
    internal class UserInterface : IUserInterface
    {
        public void Message(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
        }

        public void Error(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
        }

        public string Input()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.Write("You: ");
            var message = Console.ReadLine();

            return message;
        }

        public int InputIntData()
        {
            var result = 0;
            var check = int.TryParse(Input(), out result);

            while (check == false)
            {
                Message("Re-enter the value");
                check = int.TryParse(Input(), out result);
            }

            return result;
        }

        public double InputDoubleData()
        {
            var result = 0.0;
            var check = double.TryParse(Input(), out result);

            while (check == false)
            {
                Message("Re-enter the value");
                check = double.TryParse(Input(), out result);
            }

            return result;
        }

        public decimal InputDecimalData()
        {
            var result = 0m;
            var check = decimal.TryParse(Input(), out result);

            while (check == false)
            {
                Message("Re-enter the value");
                check = decimal.TryParse(Input(), out result);
            }

            return result;
        }

    }
}

