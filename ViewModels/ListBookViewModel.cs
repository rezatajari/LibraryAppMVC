using System.ComponentModel.DataAnnotations;

namespace LibraryAppMVC.ViewModels
{
    public class ListBookViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        [EnumDataType(typeof(GenreType))]
        public GenreType Genre { get; set; }

        public enum GenreType
        {

            Fantasy = 1,
            ScienceFiction = 2,
            Biography = 3
        }
    }
}
