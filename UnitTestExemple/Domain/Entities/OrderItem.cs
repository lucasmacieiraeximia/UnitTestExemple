namespace UnitTestExemple.Domain.Entities;

public class OrderItem
{
    public OrderItem(string productName, decimal unitPrice, decimal discount, int units)
    {
        ProductName = productName;
        UnitPrice = unitPrice;
        Discount = discount;
        Units = units;
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public int Units { get; set; }
}