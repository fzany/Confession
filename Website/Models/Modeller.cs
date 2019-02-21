using Backend.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared;
using System.Collections.Generic;
using System.Linq;

namespace Website.Models
{
    public class Modeller
    {
        public List<ConfessLoader> ConfessLoaders { get; set; }
        public List<string> Categories = Logic.Categories.ToList();
        public IEnumerable<SelectListItem> ListItems = Logic.GetIEnumarablelist();
        public Confess Confess { get; set; }
        public ConfessLoader ConfessLoader { get; set; }
        public List<CommentLoader> Loaders { get; set; }
        public Comment Comment { get; set; } = new Comment() { };


    }
}
