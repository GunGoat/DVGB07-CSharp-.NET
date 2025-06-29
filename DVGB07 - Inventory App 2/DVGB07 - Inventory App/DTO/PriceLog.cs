using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVGB07_Inventory_App.DTO;

public record PriceLog(int ProductId, decimal Price, DateTime Timestamp)
{
    public PriceLog(int productId, decimal price)
        : this(productId, price, DateTime.UtcNow) { }

    public override string ToString() =>
        $"[{Timestamp}] Product ID: {ProductId}, Price: {Price:C}";
}