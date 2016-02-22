namespace LinQDemo.Model
{
    public class Customer1
    {
        public string CustomerID { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public override string ToString()
        {
            return string.Format("{0} works at {1} in {2}, {3}",
                ContactName, CompanyName, State, Country);
        }
    }
}