using System;
using System.Data.Linq;
using System.Linq;

namespace MoreLINQToSQL.Util
{
    public static class NorthWindQueries
    {
        public static Func<NorthwindDataContext, IQueryable<Customer>> CustomersInTheUSA = CompiledQuery
            .Compile((NorthwindDataContext ctx) => ctx.Customers.Where(c => c.Country == "USA"));

        public static Func<NorthwindDataContext, string, IQueryable<Customer>> CustomersInARegion = CompiledQuery
            .Compile((NorthwindDataContext ctx, string region) => ctx.Customers.Where(c => c.Region == region));

    }
}