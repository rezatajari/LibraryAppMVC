using System.ComponentModel.DataAnnotations;
using static LibraryAppMVC.Models.Book;

namespace LibraryAppMVC.ViewModels
{
    public class BookViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string Title { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Author cannot be longer than 50 characters.")]
        public string Author { get; set; }

        [Required]
        [EnumDataType(typeof(GenreType))]
        public GenreType Genre { get; set; }
    }
}
