namespace CardStorageService.Models.Requests
{
    public class UpdateClientResponse : IOperationResult
    {
        public int ClientId { get; set; }
        public int ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
