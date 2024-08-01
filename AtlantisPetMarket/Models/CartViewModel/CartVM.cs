﻿using AtlantisPetMarket.Models.CartItemVM;
using EntityLayer.Models.Concrete;

//using AtlantisPetMarket.Models.OrderVM;
using System;
using System.Collections.Generic;

namespace AtlantisPetMarket.Models.CartViewModel
{
    public class CartVM
    {
        public int Id { get; set; } // BaseEntity'den geliyor
        public DateTime CreateDateTime { get; set; }
        public int UserId { get; set; }
        public IEnumerable<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
        //      public IEnumerable<OrderVM> Orders { get; set; } = new List<OrderVM>();
        public Cart Cart { get; set; }
    }
}
