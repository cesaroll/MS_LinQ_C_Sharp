using System.Data.Linq.Mapping;

namespace LinQDemo.Model
{
    [Table(Name = "Customers")]
    public class Customer
    {
        [Column(IsPrimaryKey = true)]
        public string CustomerID { get; set; }
        [Column()]
        public string CompanyName { get; set; }
        [Column()]
        public string ContactName { get; set; }
        [Column()]
        public string Country { get; set; }
        [Column(Name = "Region")]
        public string State { get; set; }
        public override string ToString()
        {
            return string.Format("{0} works at {1} in {2}, {3}",
                ContactName, CompanyName, State, Country);
        }
    }
}