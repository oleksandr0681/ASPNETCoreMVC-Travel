using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Travel.Models
{
    public class Select
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string ApplicationUserId { get; set; } // Ім'я класа і ID дають FOREIGN KEY (зовнішній ключ).

        public ApplicationUser? ApplicationUser { get; set; } // Зв'язок з таблицею AspNetUsers

        public int PlaceId { get; set; } // Ім'я класа і ID дають FOREIGN KEY (зовнішній ключ).

        public Place? Place { get; set; } // Зв'язок з таблицею Places.
    }
}
