namespace CardStorageService.Models.Requests
{
    public class DeleteClientResponse : IOperationResult
    {
        public int ClientId { get; set; }
        public int ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
