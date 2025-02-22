﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessLayer.Abstract;
using BusinessLayer.Models.ProductVM;
using EntityLayer.DbContexts;
using EntityLayer.Models.Concrete;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace AtlantisPetMarket.Controllers
{

    public class ProductController : Controller
    {


        private readonly IProductManager<AppDbContext, Product, int> _productManager;
        private readonly ICategoryManager<AppDbContext, Category, int> _categoryManager;
        private readonly IParentCategoryManager<AppDbContext, ParentCategory, int> _parentCategoryManager;
        private readonly IMapper _mapper;
        private readonly IValidator<ProductInsertVM> _insertValidator;
        private readonly IValidator<ProductUpdateVM> _updateValidator;

        public ProductController(IProductManager<AppDbContext, Product, int> productManager,
            ICategoryManager<AppDbContext, Category, int> categoryManager, IParentCategoryManager<AppDbContext, ParentCategory, int> parentCategory, IMapper mapper, IValidator<ProductInsertVM> insertValidator, IValidator<ProductUpdateVM> updateValidator)
        {

            _productManager = productManager;
            _categoryManager = categoryManager;
            _parentCategoryManager = parentCategory;
            _mapper = mapper;
            _insertValidator = insertValidator;
            _updateValidator = updateValidator;
        }


        public IActionResult Index()
        {

            var productsVM = _productManager.GetProducts()
                                .ProjectTo<ProductListVM>(_mapper.ConfigurationProvider);

            return View(productsVM);


        }
        [HttpGet]
        public async Task<IActionResult> GetCategoriesByParentId(int parentCategoryId)
        {
            var categories = await _categoryManager.GetAllAsync(c => c.ParentCategoryId == parentCategoryId);
            return Json(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.categories = await _categoryManager.GetAllAsync(null);
            ViewBag.parentCategories = await _parentCategoryManager.GetAllAsync(null);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductInsertVM productInsertVM, string price, int parentCategoryId)
        {

            var result = await _insertValidator.ValidateAsync(productInsertVM);

            if (!result.IsValid)
            {
                foreach (var failure in result.Errors)
                {
                    ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
                }

                ViewBag.Categories = await _categoryManager.GetAllAsync(c => c.ParentCategoryId == parentCategoryId);
                ViewBag.parentCategories = await _parentCategoryManager.GetAllAsync(null);
                return View(productInsertVM);
            }

            if (!decimal.TryParse(price, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedPrice))
            {
                ViewBag.Categories = await _categoryManager.GetAllAsync(null);
                return View(productInsertVM);
            }

            productInsertVM.Price = parsedPrice;



            var product = _mapper.Map<Product>(productInsertVM);

            if (productInsertVM.ProductPhotoPath != null)
            {
                var resource = Directory.GetCurrentDirectory();
                var extension = Path.GetExtension(productInsertVM.ProductPhotoPath.FileName);
                var imagename = Guid.NewGuid() + extension;
                var savelocation = Path.Combine(resource, "wwwroot", "productimage", imagename);
                using (var stream = new FileStream(savelocation, FileMode.Create))
                {
                    await productInsertVM.ProductPhotoPath.CopyToAsync(stream);
                }
                product.ProductPhotoPath = imagename;
            }

            await _productManager.AddAsync(product);
            return RedirectToAction("Index");
        }



        [HttpGet]
        public async Task<IActionResult> Update(int id, int? parentCategoryId)
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
            ViewBag.ParentCategories = await _parentCategoryManager.GetAllAsync(null);
            ParentCategory parentCategory = await _parentCategoryManager.FindAsync(product.ParentCategoryId);
            ViewBag.ParentCategoryName = parentCategory.ParentCategoryName;

            return View(viewModel);

        }
        [HttpPost]
        public async Task<IActionResult> Update(ProductUpdateVM productUpdateVM, string price)
        {
            var result = _updateValidator.Validate(productUpdateVM);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(productUpdateVM);

            }


            if (!decimal.TryParse(price, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedPrice))
            {
                ModelState.AddModelError("Price", "Fiyat alanı geçerli bir sayı olmalıdır.");
                ViewBag.Categories = await _categoryManager.GetAllAsync(null);
                return View(productUpdateVM);
            }

            productUpdateVM.Price = parsedPrice;

            var product = _mapper.Map<Product>(productUpdateVM);
            if (productUpdateVM.ProductPhotoUpdate != null)
            {
                var resource = Directory.GetCurrentDirectory();
                var extension = Path.GetExtension(productUpdateVM.ProductPhotoUpdate.FileName);
                var imagename = Guid.NewGuid() + extension;
                var savelocation = Path.Combine(resource, "wwwroot", "productimage", imagename);
                using (var stream = new FileStream(savelocation, FileMode.Create))
                {
                    await productUpdateVM.ProductPhotoUpdate.CopyToAsync(stream);
                }
                product.ProductPhotoPath = imagename;
            }
            else
            {
                // Eğer yeni bir resim seçilmemişse, mevcut resmi kullan
                product.ProductPhotoPath = productUpdateVM.ProductPhotoPath;
            }
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
        [HttpGet]
        public async Task<IActionResult> ProductOfTheMonth()
        {
            var productsQuery = _productManager.GetAllInclude(
                x => x.IsProductOfTheMonth == true,
                x => x.Category);

            var products = await productsQuery.ToListAsync();
            if (products == null)
            {
                return NotFound();
            }
            var productVM = _mapper.Map<IEnumerable<ProductListVM>>(products);
            return View(productVM);
        }

    }
}
