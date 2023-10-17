﻿namespace UnitTestExemple.Domain.Entities;

public class Order
{
    private List<OrderItem> _items;

    public Order()
    {
        _items = new List<OrderItem>();
        Status = OrderStatus.Submitted;
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }

    public OrderStatus Status { get; set; }

    public ICollection<OrderItem> Items
    {
        get => _items;
        set
        {
            _items = value.ToList();
        }
    }

    public OrderItem GetItemWithHighestDiscount()
    {
        var item = Items
            .OrderByDescending(item => item.Discount)
            .First();

        return item;
    }

    public void AddItem(OrderItem orderItem)
    {
        // Branch
        if (orderItem.Units == 0)
        {
            return;
        }
        
        _items.Add(orderItem);
    }

    public void SetAsPaid()
    {
        Status = OrderStatus.Paid;
    }
}
