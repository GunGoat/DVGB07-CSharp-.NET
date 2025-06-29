using DVGB07_Inventory_App.Models;

namespace DVGB07_Inventory_App.Service;

public interface IDataService
{
    IReadOnlyCollection<Book> Books { get; }
    IReadOnlyCollection<Game> Games { get; }
    IReadOnlyCollection<Movie> Movies { get; }
    IReadOnlyCollection<Receipt> Receipts { get; }
    IEnumerable<Product> Products { get; }

    int NextProductId { get; }
    int NextReceiptId { get; }

    void AddProduct(Product product);
    void UpdateProduct(Product product);
    void RemoveProduct(Product product);

    void AddReceipt(Receipt receipt);
    void UpdateReceipt(Receipt receipt);
    void RemoveReceipt(Receipt receipt);
}
