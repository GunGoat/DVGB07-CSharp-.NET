using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVGB07_Inventory_App.Service;

public interface IPrintService
{
    void Print(string content, string documentName = "Document");
}