using DVGB07_Inventory_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVGB07_Inventory_App.Service;

public interface IApiService
{
    Task<(List<Product> Products, string ErrorMessage)> GetProductsAsync();
    Task<(List<Product> Products, string ErrorMessage)> UpdateStockAsync(int id, int stock);
}
