namespace CardStorageService.Models.Requests
{
    public class DeleteCardResponse : IOperationResult
    {
        public string CardId { get; set; }
        public int ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
