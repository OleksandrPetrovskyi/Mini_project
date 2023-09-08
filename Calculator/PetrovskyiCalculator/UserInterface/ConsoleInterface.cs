namespace Calculator.UserInterface
{
    internal class ConsoleInterface : IUserInterface
    {
        public char GetChar => Console.ReadKey(false).KeyChar;

        public async Task DeleteCharacters(int index, string deleteMessage)
        {
            Console.SetCursorPosition(index, Console.CursorTop);
            Console.Write(deleteMessage);
            Console.SetCursorPosition(index, Console.CursorTop);
        }

        public async Task Message(string message)
        {
            if(message.Length > 1)
                Console.SetCursorPosition(0, Console.CursorTop);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(message);
        }

        public async Task ImportantMessage(string message)
        {
            if (message.Length > 1)
                Console.SetCursorPosition(0, Console.CursorTop);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(message);
        }
    }
}
