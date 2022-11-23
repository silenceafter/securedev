using CardStorageService.Data;
using CardStorageService.Models.Requests;
using CardStorageService.Models;
using CardStorageService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CardStorageService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        #region Services
        private readonly ILogger<CardController> _logger;
        private readonly ICardRepositoryService _cardRepositoryService;
        #endregion

        #region Constructors
        public CardController(ILogger<CardController> logger,
            ICardRepositoryService cardRepositoryService)
        {
            _logger = logger;
            _cardRepositoryService = cardRepositoryService;
        }
        #endregion

        #region Public Methods
        [HttpPost("create")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult Create([FromBody] CreateCardRequest request)
        {
            try
            {
                var cardId = _cardRepositoryService.Create(new Card
                {
                    ClientId = request.ClientId,
                    CardNo = request.CardNo,
                    ExpDate = request.ExpDate,
                    CVV2 = request.CVV2
                });
                return Ok(new CreateCardResponse
                {
                    CardId = cardId.ToString()
                });
            }
            catch (Exception e)
            {
                if (e.Message.Trim().ToLower() == "client not found")
                {
                    _logger.LogError(e, "client not found.");
                    return Ok(new CreateCardResponse
                    {
                        ErrorCode = 0,
                        ErrorMessage = "client not found."
                    });
                }
                //
                _logger.LogError(e, "Create card error.");
                return Ok(new CreateCardResponse
                {
                    ErrorCode = 1012,
                    ErrorMessage = "Create card error."
                });
            }
        }

        [HttpGet("get-by-client-id")]
        [ProducesResponseType(typeof(GetCardsResponse), StatusCodes.Status200OK)]
        public IActionResult GetByClientId([FromQuery] string clientId)
        {
            try
            {
                var cards = _cardRepositoryService.GetByClientId(clientId);
                return Ok(new GetCardsResponse
                {
                    Cards = cards.Select(card => new CardDto
                    {
                        CardNo = card.CardNo,
                        CVV2 = card.CVV2,
                        Name = card.Name,
                        ExpDate = card.ExpDate.ToString("MM/yy")
                    }).ToList()
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Get cards error.");
                return Ok(new GetCardsResponse
                {
                    ErrorCode = 1013,
                    ErrorMessage = "Get cards error."
                });
            }
        }

        [HttpGet("get-by-id")]
        [ProducesResponseType(typeof(GetCardResponse), StatusCodes.Status200OK)]
        public IActionResult GetById([FromQuery] string id)
        {
            try
            {
                var card = _cardRepositoryService.GetById(id);              
                return Ok(new GetCardResponse
                {
                    Card = new CardDto()
                    {
                        CardNo = card.CardNo,
                        Name = card.Name,
                        CVV2 = card.CVV2,
                        ExpDate = card.ExpDate.ToString("MM/yy")
                    }
                });
            }
            catch (Exception e)
            {
                if (e.Message.Trim().ToLower() == "card not found")
                {
                    _logger.LogError(e, "card not found.");
                    return Ok(new GetCardResponse
                    {
                        ErrorCode = 0,
                        ErrorMessage = "card not found."
                    });
                }
                //
                _logger.LogError(e, "Get card error.");
                return Ok(new GetCardResponse
                {
                    ErrorCode = 1013,
                    ErrorMessage = "Get card error."
                });
            }
        }

        [HttpGet("get-all")]
        [ProducesResponseType(typeof(GetAllCardsResponse), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            try
            {
                var cards = _cardRepositoryService.GetAll();
                IList<CardDto>? cardsDto = new List<CardDto>();

                //cards -> cardsDto
                foreach (var card in cards)
                {
                    cardsDto.Add(new CardDto()
                    {
                        CardNo = card.CardNo,
                        Name = card.Name,
                        CVV2 = card.CVV2,
                        ExpDate = card.ExpDate.ToString("MM/yy")
                    });
                }
                return Ok(new GetAllCardsResponse
                {
                    Cards = cardsDto
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetAll cards error.");
                return Ok(new GetAllCardsResponse
                {
                    ErrorCode = 912,
                    ErrorMessage = "GetAll cards error."
                });
            }
        }

        [HttpPost("update")]
        [ProducesResponseType(typeof(UpdateCardResponse), StatusCodes.Status200OK)]
        public IActionResult Update([FromBody] UpdateCardRequest request)
        {
            try
            {
                var rows = _cardRepositoryService.Update(new Card
                {
                    CardId = Guid.Parse(request.CardId),
                    CardNo = request.CardNo,
                    Name = request.Name,
                    CVV2 = request.CVV2,
                    ExpDate = request.ExpDate
                });
                //
                if (rows > 0)
                {
                    return Ok(new UpdateCardResponse
                    {
                        CardId = request.CardId
                    });
                }
                throw new Exception("Update card error.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Update card error.");
                return Ok(new UpdateCardResponse
                {
                    ErrorCode = 912,
                    ErrorMessage = "Update card error."
                });
            }
        }

        [HttpPost("delete")]
        [ProducesResponseType(typeof(DeleteCardResponse), StatusCodes.Status200OK)]
        public IActionResult Delete(string id)
        {
            return Ok();
        }
        #endregion
    }
}
