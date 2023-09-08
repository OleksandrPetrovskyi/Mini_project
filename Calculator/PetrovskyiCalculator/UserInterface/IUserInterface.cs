namespace Calculator.UserInterface
{
    internal interface IUserInterface
    {
        char GetChar { get; }
        Task DeleteCharacters(int index, string deleteMessage);
        Task Message(string message);
        Task ImportantMessage(string message);
    }
}
