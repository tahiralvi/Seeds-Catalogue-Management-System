using SeedsProject.Models;
using System.Collections.Generic;

namespace SeedsProject.ViewModels
{
    public class HomeIndexViewModel
    {
        public IEnumerable<Seed> Featured { get; set; } = new List<Seed>();
        public IEnumerable<Seed> Popular { get; set; } = new List<Seed>();
        public IEnumerable<Seed> Seeds { get; set; } = new List<Seed>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    }
}