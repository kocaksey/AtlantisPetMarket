﻿using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.Models.CartViewModel;
using EntityLayer.DbContexts;
using EntityLayer.Models.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace AtlantisPetMarket.ViewComponents.FishFoods
{
    public class FishFoodsList : ViewComponent
    {
        private readonly IProductManager<AppDbContext, Product, int> _productManager;
        private readonly IMapper _mapper;
        public FishFoodsList(IProductManager<AppDbContext, Product, int> productManager, IMapper mapper)
        {
            _productManager = productManager;
            _mapper = mapper;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var products = await _productManager.GetProductsByCategoryAsync(
               p => p.ParentCategory.ParentCategoryName.ToLower() == "balık" && p.Category.CategoryName.ToLower().Contains("yem"),
               p => p.ParentCategory,
               p => p.Category
           );
            var random = new Random();
            var randomProducts = products.OrderBy(x => random.Next()).Take(8).ToList();
            var model = _mapper.Map<List<ProductCartVM>>(randomProducts);
            return View(model);
        }
    }
}
