using System.ComponentModel;
using System.Runtime.InteropServices;

namespace CardStorageService.Models.Requests
{
    public class UpdateClientRequest
    {        
        public int ClientId { get; set; }
        [DefaultValue("")]
        public string? Surname { get; set; }
        [DefaultValue("")]
        public string? FirstName { get; set; }
        [DefaultValue("")]
        public string? Patronymic { get; set; }
    }
}
