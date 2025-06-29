using DVGB07_Inventory_App.DTO;
using DVGB07_Inventory_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVGB07_Inventory_App.Service;

public interface IStorageService
{
    void Save(StorageData data);
    StorageData Load();
}
