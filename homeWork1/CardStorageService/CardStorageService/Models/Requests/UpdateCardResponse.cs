namespace CardStorageService.Models.Requests
{
    public class UpdateCardResponse : IOperationResult
    {
        public string CardId { get; set; }
        public int ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
