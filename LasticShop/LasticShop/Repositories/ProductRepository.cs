using LasticShop.DatabaseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LasticShop.Repositories
{
    public class ProductRepository
    {
        ShopEntities _context;
        public ProductRepository(ShopEntities context) 
        {
            _context = context;
        }

        public async Task<List<Product>> GetProducts(int page)
        {
            var startElements = page * 20;

            return await _context.Products.Skip(page).Take(startElements).ToListAsync<Product>();
        }

        public async Task<Product> GetProductById(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<int> CreateProduct(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return product.Id;
        }

        public async Task<int> UpdateProduct(Product product)
        {
            var oldProduct = await GetProductById(product.Id);

            oldProduct = product;

            await _context.SaveChangesAsync();

            return product.Id;
        }

        public async Task<List<Review>> AddReview(int productId, Review review)
        {
            var product = await GetProductById(productId);

            if (product == null)
                return null;

            product.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return product.Reviews.ToList();
        }

        public async Task<int> DeleteProduct(int id)
        {
            var product = await GetProductById(id);

            if (product == null)
                throw new Exception("Product was not find");

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return product.Id;
        }

    }
}
