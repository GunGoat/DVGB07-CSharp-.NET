using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVGB07_Calculator_App.Models;

public record HistoryEntry(long Operand1, string Operator, long Operand2, long Result)
{
    public string Equation => $"{Operand1} {Operator} {Operand2} =";
    public override string ToString()
    {
        return $"{Operand1} {Operator} {Operand2} = {Result}";
    }
}