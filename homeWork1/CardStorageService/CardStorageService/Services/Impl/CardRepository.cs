using CardStorageService.Data;
using CardStorageService.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace CardStorageService.Services.Impl
{
    public class CardRepository : ICardRepositoryService
    {
        #region Services
        private readonly CardStorageServiceDbContext _context;
        private readonly ILogger<ClientRepository> _logger;
        private readonly IOptions<DatabaseOptions> _databaseOptions;
        #endregion

        #region Constructors
        public CardRepository(
            ILogger<ClientRepository> logger,
            CardStorageServiceDbContext context,
            IOptions<DatabaseOptions> databaseOptions)
        {
            _logger = logger;
            _context = context;
            _databaseOptions = databaseOptions;
        }
        #endregion

        public string Create(Card data)
        {
            var client = _context.Clients.FirstOrDefault(client => client.ClientId == data.ClientId);
            if (client == null)
                throw new Exception("Client not found");
            //
            _context.Cards.Add(data);
            _context.SaveChanges();
            return data.CardId.ToString();
        }

        public IList<Card> GetByClientId(string id)
        {
            List<Card> cards = new List<Card>();
            using (SqlConnection sqlConnection = new SqlConnection(_databaseOptions.Value.ConnectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = new SqlCommand(String.Format("SELECT * FROM Cards WHERE ClientId = {0}", id), sqlConnection))
                {
                    var reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        cards.Add(new Card
                        {
                            CardId = new Guid(reader["CardId"].ToString()),
                            CardNo = reader["CardNo"]?.ToString(),
                            Name = reader["Name"]?.ToString(),
                            CVV2 = reader["CVV2"]?.ToString(),
                            ExpDate = Convert.ToDateTime(reader["ExpDate"])
                        });
                    }
                }

            }
            return cards;
        }

        public int Delete(string id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_databaseOptions.Value.ConnectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = new SqlCommand(
                    String.Format($"DELETE FROM Cards WHERE CardId = '{id.Trim().ToUpper()}'"), sqlConnection))
                {
                    return sqlCommand.ExecuteNonQuery();
                }//добавить join
            }
        }

        public IList<Card> GetAll()
        {
            IList<Card> cards = new List<Card>();
            using (SqlConnection sqlConnection = new SqlConnection(_databaseOptions.Value.ConnectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = new SqlCommand(String.Format("SELECT * FROM Cards"), sqlConnection))
                {
                    var reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        cards.Add(new Card()
                        {
                            CardId = (Guid)reader["CardId"],
                            CardNo = reader["CardNo"]?.ToString(),
                            Name = reader["Name"]?.ToString(),
                            CVV2 = reader["CVV2"]?.ToString(),
                            ExpDate = Convert.ToDateTime(reader["ExpDate"])
                        });
                    }
                }
            }
            return cards;
        }

        public int Update(Card data)
        {
            if (data != null)
            {
                using (SqlConnection sqlConnection = new SqlConnection(_databaseOptions.Value.ConnectionString))
                {
                    sqlConnection.Open();                    
                    using (var sqlCommand = new SqlCommand(String.Format(
                        $"UPDATE Cards SET " +
                            $"CardNo = '{data.CardNo}', " +
                            $"Name = '{data.Name}', " +
                            $"CVV2 = '{data.CVV2}', " +
                            $"ExpDate = '{data.ExpDate.ToString()}' " +
                        $"WHERE CardId = '{data.CardId.ToString().Trim().ToUpper()}'"), sqlConnection))
                    {
                        return sqlCommand.ExecuteNonQuery();
                    }
                }
            }
            return 0;
        }

        public Card GetById(string id)
        {
            Card? card = null;
            using (SqlConnection sqlConnection = new SqlConnection(_databaseOptions.Value.ConnectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = new SqlCommand(String.Format("SELECT * FROM Cards WHERE ClientId = {0}", id), sqlConnection))
                {
                    var reader = sqlCommand.ExecuteReader();                    
                    if (reader.Read())
                    {
                        card = new Card()
                        {
                            CardId = (Guid)reader["CardId"],
                            CardNo = reader["CardNo"]?.ToString(),
                            Name = reader["Name"]?.ToString(),
                            CVV2 = reader["CVV2"]?.ToString(),
                            ExpDate = Convert.ToDateTime(reader["ExpDate"])
                        };
                    }
                }
            }
            //
            if (card == null)
                throw new Exception("Card not found");
            return card;
        }
    }
}
