using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core.Entities;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.Product_Specs;

namespace Talabat.APIs.Controllers
{
  
    public class ProductsController : BaseApiController
    {
        private readonly IProductService _productService;

        //private readonly IGenericRepository<Product> _productsRepo;
        //private readonly IGenericRepository<ProductBrand> _brandsRepo;
        //private readonly IGenericRepository<ProductCategory> _categoriesRepo;
        private readonly IMapper _mapper;

        public ProductsController(
            //IGenericRepository<Product>productsRepo,
        //    IGenericRepository<ProductBrand>brandsRepo,
        //    IGenericRepository<ProductCategory>categoriesRepo,
        IProductService productService,
            IMapper mapper)
        {
            _productService = productService;
            //_productsRepo = productsRepo;
            //_brandsRepo = brandsRepo;
            //_categoriesRepo = categoriesRepo;
            _mapper = mapper;
        }

        // /api/Products
        //[Authorize/*(AuthenticationSchemes = "X")*/]
        [CachedAttribute(600)] //Action filter
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpeceficationsParams speceficationsParams)
        {
            var products=await _productService.GetProductsAsync(speceficationsParams);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);


            var count= await _productService.GetCountAsync(speceficationsParams);   

            return Ok(new Pagination<ProductToReturnDto>(speceficationsParams.PageIndex,  speceficationsParams.PageSize,count, data));
        }

        // /api/Products/1
        [ProducesResponseType(typeof(ProductToReturnDto),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>>GetProduct(int id)
        {

            var product =await _productService.GetProductAsync(id);

            if (product == null)
                return NotFound(new {Message="Not Found" , StatusCode=404});

            return Ok(_mapper.Map<Product,ProductToReturnDto>(product));
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var brands = await _productService.GetBrandsAsync();
            return Ok(brands);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IReadOnlyList<ProductCategory>>> GetCategories()
        {
            var categories = await _productService.GetCategoriesAsync();
            return Ok(categories);
        }
    }
}
