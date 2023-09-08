namespace Calculator.Solution_methods
{
    public class SolutionResponse
    {
        public SolutionResponse(bool success, string request, string response)
        {
            Success = success;
            Request = request;
            Response = response;
        }

        public bool Success { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
    }
}
