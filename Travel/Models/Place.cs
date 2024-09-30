using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Travel.Models
{
    [Display(Name = "Місце")]
    public class Place
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string ApplicationUserId { get; set; } // Ім'я класа і ID дають FOREIGN KEY (зовнішній ключ).

        public ApplicationUser? ApplicationUser { get; set; } // Зв'язок з таблицею AspNetUsers

        public byte[]? Data { get; set; }

        [MaxLength(50)]
        public string? PictureMimeType { get; set; }

        public string? FileName { get; set; }

        [MaxLength(100), Display(Name = "Назва")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100), Display(Name = "Країна")]
        public string? Country { get; set; }

        [MaxLength(100), Display(Name = "Місто")]
        public string? City { get; set; }

        [MaxLength(250), Display(Name = "Адреса")]
        public string? Address { get; set; }

        [MaxLength(2000), Display(Name = "Опис")]
        public string? Description { get; set; } = string.Empty;

        [Display(Name = "Підтверджено")]
        public bool IsConfirmed { get; set; }

        [Display(Name = "Дата й час створеня")]
        public DateTime? Created { get; set; } = DateTime.Now;

        [Display(Name = "Оцінки")]
        public ICollection<Mark>? Marks { get; set; }
    }
}
