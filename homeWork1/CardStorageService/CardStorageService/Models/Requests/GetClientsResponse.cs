namespace CardStorageService.Models.Requests
{
    public class GetClientsResponse : IOperationResult
    {
        public IList<ClientDto>? Clients { get; set; }
        public int ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
    }
}