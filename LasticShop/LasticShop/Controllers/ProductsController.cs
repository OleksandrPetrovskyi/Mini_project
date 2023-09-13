using LasticShop.DatabaseModels;
using LasticShop.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LasticShop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : Controller
    {
        ProductRepository _productRepository { get; set; }

        public ProductsController(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(int page = 0)
        {
            var products = await _productRepository.GetProducts(page);

            if (products == null)
                return NotFound();

            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productRepository.GetProductById(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            var newProductId = await _productRepository.CreateProduct(product);

            if (newProductId == 0)
                return NotFound();

            return Ok(newProductId);
        }
        
        [HttpPut]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            try
            {
                _productRepository.UpdateProduct(product);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }

            return Ok(product.Id);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int productId, Review review)
        {
            var reviews = _productRepository.AddReview(productId, review);

            if (reviews == null)
                return NotFound("The product was not find");

            return Ok(reviews);
        }

        [HttpDelete ("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var productId = 0;
            try
            {
                productId = await _productRepository.DeleteProduct(id);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(productId);
        }
    }
}
