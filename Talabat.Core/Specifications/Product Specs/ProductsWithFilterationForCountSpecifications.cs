using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.Product_Specs
{
    public class ProductsWithFilterationForCountSpecifications : BaseSpecifications<Product>
    {
        public ProductsWithFilterationForCountSpecifications(ProductSpeceficationsParams speceficationsParams) :
              base(p =>
            (string.IsNullOrEmpty(speceficationsParams.Search) || p.Name.ToLower().Contains(speceficationsParams.Search) &&
            (!speceficationsParams.brandId.HasValue || p.BrandId == speceficationsParams.brandId.Value) &&
            (!speceficationsParams.categoryId.HasValue || p.CategoryId == speceficationsParams.categoryId.Value)

            ))
        {

        }
    }
}
