using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.Product_Specs;

namespace Talabat.Service;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<ProductBrand>> GetBrandsAsync()
           => await _unitOfWork.Repository<ProductBrand>().GetAllAsync();


    public async Task<IReadOnlyList<ProductCategory>> GetCategoriesAsync()
        =>await _unitOfWork.Repository<ProductCategory>().GetAllAsync();

    public async Task<Product?> GetProductAsync(int productId)
    {
        var spec = new ProductWithBrandAndCategorySpecifications(productId);

        var product = await _unitOfWork.Repository<Product>().GetByIdWithSpecAsync(spec);

        return product;
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(ProductSpeceficationsParams speceficationsParams)
    {
        var spec = new ProductWithBrandAndCategorySpecifications(speceficationsParams);

        var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);

        return products;
    }

    public async Task<int> GetCountAsync(ProductSpeceficationsParams speceficationsParams)
    {

        var countSpec = new ProductsWithFilterationForCountSpecifications(speceficationsParams);

        var count = await _unitOfWork.Repository<Product>().GetCountAsync(countSpec);

        return count;
    }
}
