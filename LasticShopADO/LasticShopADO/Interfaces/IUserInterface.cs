namespace LasticShopAdo.Interfaces
{
    internal interface IUserInterface
    {
        public void Message(string message);
        public void Error(string error);
        public string Input();
        public double InputDoubleData();
        public decimal InputDecimalData();
        public int InputIntData();


    }
}

