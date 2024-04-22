using ManagerIO.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerApi.Model.DM
{
    public class UsersDM
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string? office { get; set; }
        public string password { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTipe { get; set; }

    }
}
