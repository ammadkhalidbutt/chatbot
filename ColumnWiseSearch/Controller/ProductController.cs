using ColumnWiseSearch.Models;
using ColumnWiseSearch.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ColumnWiseSearch.Controller
{
    // Controller
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResult<Product>>> SearchProducts([FromBody] SearchRequest request)
        {
            var result = await _productRepository.SearchProductsAsync(request);
            return Ok(result);
        }
    }
}
