namespace WebAPI.Models
{
    public class ResponseBase<T>
    {
        public ResponseBase()
        {
            Data = default(T);
        }
        public ResponseBase(T value)
        {
            Data = value;
        }

        public T Data { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
    }
}
