using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.Product_Specs
{
    public class ProductWithBrandAndCategorySpecifications:BaseSpecifications<Product>
    {
        public ProductWithBrandAndCategorySpecifications(ProductSpeceficationsParams speceficationsParams)
            : base(p=>
            (string.IsNullOrEmpty(speceficationsParams.Search) || p.Name.ToLower().Contains(speceficationsParams.Search) &&
            (!speceficationsParams.brandId.HasValue || p.BrandId == speceficationsParams.brandId.Value) &&
            (!speceficationsParams.categoryId.HasValue || p.CategoryId == speceficationsParams.categoryId.Value)

            ))
            
        {
            AddIncludes();

            if(!string.IsNullOrEmpty(speceficationsParams.sort))
            {
                switch (speceficationsParams.sort)
                {
                    case "priceAsc":
                        AddOrderBy(p => p.Price);
                        break;

                    case "priceDesc":
                        AddOrderByDesc(p => p.Price);
                        break;

                    default:
                        AddOrderBy(p => p.Name);
                        break;
                }
            }
            else
                AddOrderBy(p => p.Name);

            ApplyPagination((speceficationsParams.PageIndex - 1) * speceficationsParams.PageSize, speceficationsParams.PageSize);
        }

        public ProductWithBrandAndCategorySpecifications(int id)
            :base(p=>p.Id==id)
        {
            AddIncludes();
        }

        private void AddIncludes()
        {
            Includes.Add(p => p.Brand);
            Includes.Add(p => p.Category);
        }

    }
}
