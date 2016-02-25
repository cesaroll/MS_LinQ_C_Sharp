using System.Data.Linq.Mapping;

namespace LinQDemo.Model
{
    [Table(Name = "Products")]
    public class Product
    {
        [Column(IsPrimaryKey = true)]
        public int ProductID { get; set; }
        [Column()]
        public string ProductName { get; set; }
        [Column()]
        public int CategoryID { get; set; }
        [Column()]
        public decimal UnitPrice { get; set; }
        [Column()]
        public short UnitsInStock { get; set; }
        [Column()]
        public bool Discontinued { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1})", ProductName, ProductID);
        }
    }
}