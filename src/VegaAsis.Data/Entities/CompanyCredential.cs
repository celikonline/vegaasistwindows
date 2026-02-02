using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VegaAsis.Data.Entities
{
    /// <summary>
    /// Şirket portal giriş bilgileri (kullanıcı bazlı).
    /// Her kullanıcı için her şirkete ait ayrı credential kaydı tutulur.
    /// </summary>
    [Table("company_credentials", Schema = "public")]
    public class CompanyCredential
    {
        [Column("id")]
        public Guid Id { get; set; }

        /// <summary>Şirket kodu (ak, anadolu, allianz vb.)</summary>
        [Column("company_id")]
        public string CompanyId { get; set; }

        /// <summary>Portal kullanıcı adı</summary>
        [Column("username")]
        public string Username { get; set; }

        /// <summary>Şifrelenmiş parola</summary>
        [Column("password_encrypted")]
        public string PasswordEncrypted { get; set; }

        /// <summary>Bu credential'ın sahibi olan kullanıcı</summary>
        [Column("user_id")]
        public Guid? UserId { get; set; }

        /// <summary>Aktif/pasif durumu</summary>
        [Column("is_active")]
        public bool IsActive { get; set; }

        /// <summary>Oluşturulma tarihi</summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>Güncellenme tarihi</summary>
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
