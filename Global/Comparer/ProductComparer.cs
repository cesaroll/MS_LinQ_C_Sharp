using System.Collections.Generic;
using LinQDemo.Model;

namespace Global.Comparer
{
    public class ProductComparer : IEqualityComparer<Product>
    {
        public bool Equals(Product x, Product y)
        {
            return x.ProductID.Equals(y.ProductID);
        }

        public int GetHashCode(Product obj)
        {
            return obj.GetHashCode();
        }
    }
}