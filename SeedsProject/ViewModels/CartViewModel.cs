using System;
using System.Collections.Generic;
using System.Linq;

namespace SeedsProject.ViewModels
{
    public class CartItemViewModel
    {
        public int SeedId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int Stock { get; set; }
        public string Image { get; set; }
        public decimal SubTotal => Price * Quantity;
    }

    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
        public decimal Total => Items?.Sum(i => i.SubTotal) ?? 0m;
    }
}