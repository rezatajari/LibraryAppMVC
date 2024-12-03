using static LibraryAppMVC.Models.Book;

namespace LibraryAppMVC.ViewModels
{
    public class AddBookViewModel
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public GenreType Genre { get; set; }
    }

}
