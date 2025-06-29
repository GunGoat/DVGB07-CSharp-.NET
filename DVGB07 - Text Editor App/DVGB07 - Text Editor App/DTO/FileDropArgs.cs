using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DVGB07_Text_Editor_App.DTO;

/// <summary>
/// Contains the arguments for the file drop event.
/// </summary>
public class FileDropArgs
{
    /// <summary>
    /// Full file path of the dropped file.
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// The modifier keys <see cref="ModifierKeys"/> pressed during the drop operation.
    /// </summary>
    public ModifierKeys ModifierKeys { get; set; }

    /// <summary>
    /// The position of the caret in the target control at the time of the drop.
    /// (The number of characters to reach that point in the text.)
    /// </summary>
    public int CaretIndex { get; set; }
}