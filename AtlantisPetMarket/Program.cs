
using BusinessLayer.Abstract;
using BusinessLayer.AutoMapperConfig;
using BusinessLayer.Concrete;
using BusinessLayer.Models.CategoryVM;
using BusinessLayer.Models.ContactVM;
using BusinessLayer.Models.ProductVM;
using BusinessLayer.Models.SocialMediaVM;
using BusinessLayer.ValidationsRules.CategoryValidator;
using BusinessLayer.ValidationsRules.ContactValidator;
using BusinessLayer.ValidationsRules.ProductValidator;
using BusinessLayer.ValidationsRules.SocialMediaValidator;
using EntityLayer.DbContexts;
using EntityLayer.Models.Concrete;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")));

// Managers and services
builder.Services.AddScoped<IProductManager<AppDbContext, Product, int>, ProductManager<AppDbContext, Product, int>>();
builder.Services.AddScoped<ICategoryManager<AppDbContext, Category, int>, CategoryManager<AppDbContext, Category, int>>();
builder.Services.AddScoped<IParentCategoryManager<AppDbContext, ParentCategory, int>, ParentCategoryManager<AppDbContext, ParentCategory, int>>();
builder.Services.AddScoped<IContactManager<AppDbContext, Contact, int>, ContactManager<AppDbContext, Contact, int>>();
builder.Services.AddScoped<ISocialMediaManager<AppDbContext, SocialMedia, int>, SocialMediaManager<AppDbContext, SocialMedia, int>>();
builder.Services.AddScoped<ICartManager<AppDbContext, Cart, int>, CartManager<AppDbContext, Cart, int>>();
builder.Services.AddScoped<ICartItemManager<AppDbContext, CartItem, int>, CartItemManager<AppDbContext, CartItem, int>>();
builder.Services.AddScoped<IOrderManager<AppDbContext, Order, int>, OrderManager<AppDbContext, Order, int>>();
builder.Services.AddScoped<IOrderItemManager<AppDbContext, OrderItem, int>, OrderItemManager<AppDbContext, OrderItem, int>>();
builder.Services.AddScoped<IMessageManager<AppDbContext, Message, int>, MessageManager<AppDbContext, Message, int>>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MapperConfig));

// Controllers and views
builder.Services.AddControllersWithViews(options =>
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

// FluentValidation
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddTransient<IValidator<ProductInsertVM>, ProductInsertValidator>();
builder.Services.AddTransient<IValidator<ProductUpdateVM>, ProductUpdateValidator>();
builder.Services.AddTransient<IValidator<CategoryUpdateVM>, CategoryUpdateValidator>();
builder.Services.AddTransient<IValidator<CategoryInsertVM>, CategoryInsertValidator>();
builder.Services.AddTransient<IValidator<ContactInsertVM>, ContactInsertValidator>();
builder.Services.AddTransient<IValidator<ContactUpdateVM>, ContactUpdateValidator>();
builder.Services.AddTransient<IValidator<SocialMediaUpdateVM>, SocialMediaUpdateValidator>();
builder.Services.AddTransient<IValidator<SocialMediaInsertVM>, SocialMediaInsertValidator>();

// Identity
builder.Services.AddIdentity<User, UserRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

var app = builder.Build();

// Localization settings
var cultureInfo = new CultureInfo("tr-TR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // Add authentication middleware
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
