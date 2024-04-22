using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagerApi.Model
{
    [Table("tb_users")]
    public class Users
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string name { get; set; }

        [Column("office")]
        public string? office { get; set; }

        [Column("password")]
        public string password { get; set; }

        [Column("refresh_token")]
        public string RefreshToken { get; set; }

        [Column("refresh_token_expiry_time")]
        public DateTime RefreshTokenExpiryTime { get; set; }

    }
}
