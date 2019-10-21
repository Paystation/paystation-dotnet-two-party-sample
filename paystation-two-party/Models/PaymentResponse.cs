namespace paystation_two_party.Models
{
    public class PaymentResponse
    {
        public string ResponseXml { get; set; }

        public string Code { get; set; }

        public string Message { get; set; }

        public bool IsError { get; set; }
    }
}