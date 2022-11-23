using CardStorageService.Data;
using CardStorageService.Models;
using Castle.Core.Internal;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace CardStorageService.Services.Impl
{
    public class ClientRepository : IClientRepositoryService
    {
        #region Services
        private readonly CardStorageServiceDbContext _context;
        private readonly ILogger<ClientRepository> _logger;
        private readonly IOptions<DatabaseOptions> _databaseOptions;
        #endregion

        #region Constructors
        public ClientRepository(
            ILogger<ClientRepository> logger,
            CardStorageServiceDbContext context,
            IOptions<DatabaseOptions> databaseOptions)
        {
            _logger = logger;
            _context = context;
            _databaseOptions = databaseOptions;            
    }

        public int Create(Client data)
        {
            _context.Clients.Add(data);
            _context.SaveChanges();
            return data.ClientId;
        }

        public int Update(Client data)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_databaseOptions.Value.ConnectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = new SqlCommand(String.Format($"UPDATE Clients " +
                    $"SET Surname = '{data.Surname}', " +
                        $"FirstName = '{data.FirstName}', " +
                        $"Patronymic = '{data.Patronymic}' " +
                        $"WHERE ClientId = {data.ClientId}"), sqlConnection))
                {
                    sqlCommand.ExecuteNonQuery();
                    return data.ClientId;
                }
            }
            return 0;
        }

        public int Delete(int id)
        {            
            using (SqlConnection sqlConnection = new SqlConnection(_databaseOptions.Value.ConnectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = new SqlCommand(String.Format($"DELETE FROM Clients WHERE ClientId = {id}"), sqlConnection))
                {
                    sqlCommand.ExecuteNonQuery();
                    return id;
                }
            }
            return 0;
        }

        public IList<Client> GetAll()
        {          
            IList<Client> clients = new List<Client>();
            using (SqlConnection sqlConnection = new SqlConnection(_databaseOptions.Value.ConnectionString))
            {
                sqlConnection.Open();                
                //clients
                using (var sqlCommand = new SqlCommand(String.Format("SELECT * FROM Clients"), sqlConnection))
                {
                    var reader = sqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            clients.Add(new Client()
                            {
                                ClientId = (int)reader["ClientId"],
                                Surname = reader["Surname"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                Patronymic = reader["Patronymic"].ToString()
                            });
                        }
                    }
                    reader.Close();
                }

                //cards
                foreach (var client in clients)
                {
                    ICollection<Card> cards = new HashSet<Card>();
                    using (var sqlCommand = new SqlCommand(String.Format("SELECT * FROM Cards WHERE ClientId = {0}", client.ClientId), sqlConnection))
                    {
                        var reader = sqlCommand.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                cards.Add(new Card()
                                {
                                    ClientId = (int)reader["ClientId"],
                                    CardId = (Guid)reader["CardId"],
                                    CardNo = reader["CardNo"]?.ToString(),
                                    Name = reader["Name"]?.ToString(),
                                    CVV2 = reader["CVV2"]?.ToString(),
                                    ExpDate = Convert.ToDateTime(reader["ExpDate"])
                                });
                            }
                        }
                        reader.Close();
                        client.Cards = cards;
                    }
                }
            }
            return clients;
        }

        public Client GetById(int id)
        {           
            using (SqlConnection sqlConnection = new SqlConnection(_databaseOptions.Value.ConnectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = new SqlCommand(String.Format("SELECT Clients.ClientId, Clients.Surname, Clients.FirstName, " +
                    "Clients.Patronymic, Cards.ClientId AS \"CardsClientId\", Cards.CardId, Cards.CardNo, Cards.Name, Cards.CVV2, Cards.ExpDate " +
                    "FROM Clients " +
                    "LEFT JOIN Cards " +
                    "ON Clients.ClientId = Cards.ClientId " +
                    "WHERE Clients.ClientId = {0}", id), sqlConnection))
                {
                    var reader = sqlCommand.ExecuteReader();
                    ICollection<Card> cards = new HashSet<Card>();
                    //
                    if (reader.HasRows)
                    {
                        bool cMain = false;
                        int clientId = 0;
                        string surname = "";
                        string firstName = "";
                        string patronymic = "";
                        //
                        while (reader.Read())
                        {
                            if (!cMain)
                            {
                                clientId = (int)reader["ClientId"];
                                surname = reader["Surname"].ToString();
                                firstName = reader["FirstName"].ToString();
                                patronymic = reader["Patronymic"].ToString();
                                cMain = true;
                            }
                           
                            if (!reader["CardId"].ToString().IsNullOrEmpty())
                            {
                                cards.Add(new Card()
                                {
                                    ClientId = (int)reader["CardsClientId"],
                                    CardId = (Guid)reader["CardId"],
                                    CardNo = reader["CardNo"]?.ToString(),
                                    Name = reader["Name"]?.ToString(),
                                    CVV2 = reader["CVV2"]?.ToString(),
                                    ExpDate = Convert.ToDateTime(reader["ExpDate"])
                                });
                            }                                                    
                        }                        
                        
                        //client
                        return new Client()
                        {
                            ClientId = clientId,
                            Surname = surname,
                            FirstName = firstName,
                            Patronymic = patronymic,
                            Cards = cards
                        };
                    }                    
                }
            }
            throw new Exception("Client not found");
        }
        #endregion
    }
}
