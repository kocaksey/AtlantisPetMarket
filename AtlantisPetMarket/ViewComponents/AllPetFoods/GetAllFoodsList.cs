﻿using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.Models.CartViewModel;
using EntityLayer.DbContexts;
using EntityLayer.Models.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace AtlantisPetMarket.ViewComponents.AllPetFoods
{

    public class GetAllFoodsList : ViewComponent
    {
        private readonly IProductManager<AppDbContext, Product, int> _productManager;
        private readonly IMapper _mapper;
        public GetAllFoodsList(IProductManager<AppDbContext, Product, int> productManager, IMapper mapper)
        {
            _productManager = productManager;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var products = await _productManager.GetAllAsync(p => p.Category.CategoryName.ToLower().Contains("mama"));


            var random = new Random();
            var randomProducts = products.OrderBy(x => random.Next()).Take(12).ToList();


            var model = _mapper.Map<List<ProductCartVM>>(randomProducts);

            return View(model);
        }
    }
}
