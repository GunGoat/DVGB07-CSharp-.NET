using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVGB07_Inventory_App.DTO;

public record StockLog(int ProductId, int Stock, DateTime Timestamp)
{
    public StockLog(int productId, int stock)
        : this(productId, stock, DateTime.UtcNow) { }

    public override string ToString() =>
        $"[{Timestamp}] Product ID: {ProductId}, New Stock: {Stock}";
}