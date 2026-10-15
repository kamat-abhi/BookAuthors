using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookAuthors.ViewModels
{
    public class BookFormViewModel
    {
        public int BookId { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Authors")]
        public List<int> SelectedAuthorIds { get; set; } = new();

        public List<SelectListItem> AllAuthors { get; set; } = new();
    }
}
