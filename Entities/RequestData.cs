namespace WebApi.Entities
{
    public class RequestData
    {
        public int Nonce { get; set; }
        public int Timestamp { get; set; }
        public string Signature { get; set; }
    }
}