﻿using AtlantisPetMarket.Models.ProductVM;
using AtlantisPetMarket.ValidationsRules;
using AutoMapper;
using BusinessLayer.Abstract;
using EntityLayer.DbContexts;
using EntityLayer.Models.Concrete;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;


namespace AtlantisPetMarket.Controllers
{

    public class ProductController : Controller
    {

        private readonly IProductManager<AppDbContext, Product, int> _productManager;
        private readonly ICategoryManager<AppDbContext, Category, int> _categoryManager;
        private readonly IMapper _mapper;
        private readonly IValidator<ProductUpdateVM> _validator;
        public ProductController(IProductManager<AppDbContext, Product, int> productManager,
            ICategoryManager<AppDbContext, Category, int> categoryManager, IMapper mapper, IValidator<ProductUpdateVM> validator)
        {
            _productManager = productManager;
            _categoryManager = categoryManager;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<ActionResult<IEnumerable<Product>>> Index(int id)
        {
            var products = await _productManager.GetAllIncludeAsync(x => x.CategoryId == id, x => x.Category);
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductInsertVM productVM, string price)
        {
            if (!decimal.TryParse(price, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedPrice))
            {
                ModelState.AddModelError("Price", "Fiyat alanı geçerli bir sayı olmalıdır.");
                ViewBag.Categories = await _categoryManager.GetAllAsync(null);
                return View(productVM);
            }

            productVM.Price = parsedPrice;

            // Doğrulama
            var validator = new ProductValidatorInsert();
            ValidationResult results = await validator.ValidateAsync(productVM);

            if (!results.IsValid)
            {
                foreach (var failure in results.Errors)
                {
                    ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
                }
                ViewBag.Categories = await _categoryManager.GetAllAsync(null);
                return View(productVM);
            }

            var product = _mapper.Map<Product>(productVM);
            await _productManager.AddAsync(product);
            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productManager.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var viewModel = _mapper.Map<ProductUpdateVM>(product);
            ViewBag.Categories = await _categoryManager.GetAllAsync(null);

            Category category = await _categoryManager.FindAsync(product.CategoryId);
            ViewBag.CategoryName = category.CategoryName;
            return View(viewModel);

        }
        [HttpPost]
        public async Task<IActionResult> Update(ProductUpdateVM model, string price, int id)
        {
            if (!decimal.TryParse(price, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedPrice))
            {
                ModelState.AddModelError("Price", "Fiyat alanı geçerli bir sayı olmalıdır.");
                ViewBag.Categories = await _categoryManager.GetAllAsync(null);
                return View(model);
            }

            model.Price = parsedPrice;

            // Doğrulama
            ValidationResult results = await _validator.ValidateAsync(model);

            if (!results.IsValid)
            {
                foreach (var failure in results.Errors)
                {
                    ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
                }
                ViewBag.Categories = await _categoryManager.GetAllAsync(null);
                return View(model);
            }

            var product = await _productManager.FindAsync(model.Id);
            if (product == null)
            {
                return NotFound();
            }

            product = _mapper.Map(model, product);
            await _productManager.UpdateAsync(product);
            return RedirectToAction(nameof(Index));
        }

    
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productManager.FindAsync(id);
            if (product != null)
            {
                await _productManager.DeleteAsync(product);
            }
            return RedirectToAction("Index");
        }
    }
}
