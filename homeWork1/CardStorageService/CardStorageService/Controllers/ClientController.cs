using CardStorageService.Data;
using CardStorageService.Models;
using CardStorageService.Models.Requests;
using CardStorageService.Requests;
using CardStorageService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography;

namespace CardStorageService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        #region Services
        private readonly IClientRepositoryService _clientRepositoryService;
        private readonly ILogger<CardController> _logger;
        #endregion


        #region Constructors
        public ClientController(
            ILogger<CardController> logger,
            IClientRepositoryService clientRepositoryService)
        {
            _logger = logger;
            _clientRepositoryService = clientRepositoryService;
        }
        #endregion

        #region Pulbic Methods
        [HttpPost("create")]
        [ProducesResponseType(typeof(CreateClientResponse), StatusCodes.Status200OK)]
        public IActionResult Create([FromBody] CreateClientRequest request)
        {
            try
            {
                var clientId = _clientRepositoryService.Create(new Client
                {
                    FirstName = request.FirstName,
                    Surname = request.Surname,
                    Patronymic = request.Patronymic
                });
                return Ok(new CreateClientResponse
                {
                    ClientId = clientId
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Create client error.");
                return Ok(new CreateCardResponse
                {
                    ErrorCode = 912,
                    ErrorMessage = "Create client error."
                });
            }
        }

        [HttpGet("get-all")]
        [ProducesResponseType(typeof(GetClientsResponse), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            try
            {                
                var clients = _clientRepositoryService.GetAll();
                if (clients is not null)
                {             
                    IList<ClientDto>? clientsDto = new List<ClientDto>();
                    //clients -> clientsDto
                    foreach (var client in clients)
                    {
                        //cards -> cardsDto
                        ICollection<CardDto> cardsDto = new HashSet<CardDto>();
                        foreach(var card in client.Cards)
                        {
                            cardsDto.Add(new CardDto()
                            {
                                CardNo = card.CardNo,
                                Name = card.Name,
                                CVV2 = card.CVV2,
                                ExpDate = card.ExpDate.ToString("MM/yy")
                            });
                        }

                        clientsDto.Add(new ClientDto()
                        {
                            Surname = client.Surname,
                            FirstName = client.FirstName,
                            Patronymic = client.Patronymic,
                            Cards = cardsDto
                        });
                    }
                    //
                    return Ok(new GetClientsResponse
                    {
                        Clients = clientsDto
                    });
                }
                throw new Exception("GetAll not found");
            }
            catch (Exception e)
            {               
                _logger.LogError(e, "GetAll client error.");
                return Ok(new GetClientsResponse
                {
                    ErrorCode = 912,
                    ErrorMessage = "GetAll client error."
                });
            }
        }

        [HttpGet("get-by-id")]
        [ProducesResponseType(typeof(GetClientByIdResponse), StatusCodes.Status200OK)]
        public IActionResult GetById(int id)
        {
            try
            {
                var client = _clientRepositoryService.GetById(id);
                if (client is not null)
                {
                    ICollection<CardDto> cardDto = new HashSet<CardDto>();
                    foreach(var card in client.Cards)
                    {
                        cardDto.Add(new CardDto()
                        {
                            CardNo = card.CardNo,
                            Name = card.Name,
                            CVV2 = card.CVV2,
                            ExpDate = card.ExpDate.ToString("MM/yy")
                        });
                    }
                    //
                    return Ok(new GetClientByIdResponse()
                    {
                        Client = new ClientDto()
                        {
                            Surname= client.Surname,
                            FirstName = client.FirstName,
                            Patronymic = client.Patronymic,
                            Cards = cardDto
                        }
                    });
                }
                throw new Exception("client not found");
            }
            catch (Exception e)
            {
                if (e.Message.Trim().ToLower() == "client not found")
                {
                    _logger.LogError(e, "client not found.");
                    return Ok(new GetClientByIdResponse
                    {
                        ErrorCode = 0,
                        ErrorMessage = "client not found."
                    });
                }
                //
                _logger.LogError(e, "GetById client error.");
                return Ok(new GetClientByIdResponse
                {
                    ErrorCode = 912,
                    ErrorMessage = "GetById client error."
                });
            }
        }

        [HttpPost("update")]
        [ProducesResponseType(typeof(UpdateClientResponse), StatusCodes.Status200OK)]
        public IActionResult Update([FromBody] UpdateClientRequest request)
        {
            try
            {
                var clientId = _clientRepositoryService.Update(new Client
                {
                    ClientId = request.ClientId,
                    FirstName = request.FirstName,
                    Surname = request.Surname,
                    Patronymic = request.Patronymic
                });
                return Ok(new UpdateClientResponse
                {
                    ClientId = clientId
                });
            }
            catch (Exception e)
            {
                if (e.Message.Trim().ToLower() == "client not found")
                {
                    _logger.LogError(e, "client not found.");
                    return Ok(new UpdateClientResponse
                    {
                        ErrorCode = 0,
                        ErrorMessage = "client not found."
                    });
                }
                //
                _logger.LogError(e, "Update client error.");
                return Ok(new UpdateClientResponse
                {
                    ErrorCode = 912,
                    ErrorMessage = "Update client error."
                });
            }
        }

        [HttpPost("delete")]
        [ProducesResponseType(typeof(DeleteClientResponse), StatusCodes.Status200OK)]
        public IActionResult Delete(int id)
        {
            try
            {
                var client = _clientRepositoryService.Delete(id);
                return Ok(new DeleteClientResponse
                {
                    ClientId = id
                });
            }
            catch (Exception e)
            {
                if (e.Message.Trim().ToLower() == "client not found")
                {
                    _logger.LogError(e, "client not found.");
                    return Ok(new DeleteClientResponse
                    {
                        ErrorCode = 0,
                        ErrorMessage = "client not found."
                    });
                }
                //
                _logger.LogError(e, "Delete client error.");
                return Ok(new DeleteClientResponse
                {
                    ErrorCode = 912,
                    ErrorMessage = "Delete client error."
                });
            }
        }
        #endregion
    }
}
