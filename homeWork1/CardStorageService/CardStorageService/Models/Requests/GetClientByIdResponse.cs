namespace CardStorageService.Models.Requests
{
    public class GetClientByIdResponse : IOperationResult
    {
        public ClientDto? Client { get; set; }
        public int ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
