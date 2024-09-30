using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Travel.Models
{
    [Display(Name = "Оцінка")]
    public class Mark
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string ApplicationUserId { get; set; } // Ім'я класа і ID дають FOREIGN KEY (зовнішній ключ).

        public ApplicationUser? ApplicationUser { get; set; } // Зв'язок з таблицею AspNetUsers

        [Range(1, 5,
            ErrorMessage = "Значення для {0} повинно бути між {1} і {2}."),
            Display(Name = "Бал")]
        public int Point {  get; set; }

        [MaxLength(2000), Display(Name = "Коментар")]
        public string? Commentary { get; set; }

        public int PlaceId { get; set; } // Ім'я класа і ID дають FOREIGN KEY (зовнішній ключ).

        public Place? Place { get; set; } // Зв'язок з таблицею Places.

        [Display(Name = "Дата й час створеня")]
        public DateTime? Created { get; set; } = DateTime.Now;
    }
}
