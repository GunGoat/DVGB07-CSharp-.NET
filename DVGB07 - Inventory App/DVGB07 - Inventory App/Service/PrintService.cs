using System;
using System.IO;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DVGB07_Inventory_App.Service;

/// <summary>
/// Implements the <see cref="IPrintService"/> interface and provides the functionality for printing content to a printer or PDF.
/// </summary>
/// <remarks>
/// This class allows printing content either to a physical printer or to a PDF using the "Microsoft Print to PDF" printer.
/// It attempts to find the default printer or the PDF printer, then formats the content into a flow document and sends it to the printer.
/// </remarks>
public class PrintService : IPrintService
{
    /// <summary>
    /// Prints the specified content to a printer or PDF.
    /// </summary>
    /// <param name="content">The content to be printed.</param>
    /// <param name="documentName">The name of the document to be printed. Default is "Document".</param>
    public void Print(string content, string documentName)
    {
        // Get the default printer or fall back to PDF printer.
        PrintQueue? printer = GetDefaultPrinter() ?? GetPdfPrinter();
        if (printer == null)
        {
            // Show error if no printer is available.
            MessageBox.Show("No available printer found. Please install a physical or PDF printer and try again.",
                            "Printing Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        // Set up the print dialog with the selected printer.
        PrintDialog printDialog = new PrintDialog
        {
            PrintQueue = printer
        };

        // Create a flow document with the provided content.
        FlowDocument doc = new FlowDocument(new Paragraph(new Run(content)))
        {
            PagePadding = new Thickness(50),
            FontSize = 12,
            FontFamily = new FontFamily("Arial")
        };
        IDocumentPaginatorSource paginator = doc;

        // Adjust settings for PDF printing (Microsoft Print to PDF).
        if (printer.Name.Contains("Microsoft Print to PDF"))
        {
            PrintTicket printTicket = printer.DefaultPrintTicket;
            printTicket.PageOrientation = PageOrientation.Portrait;
            printDialog.PrintTicket = printTicket;
        }

        // Print the document.
        printDialog.PrintDocument(paginator.DocumentPaginator, documentName);
    }

    /// <summary>
    /// Retrieves the default available printer from the system.
    /// </summary>
    /// <returns>The default <see cref="PrintQueue"/>, or <c>null</c> if no valid printer is found.</returns>
    private PrintQueue? GetDefaultPrinter()
    {
        using (PrintServer printServer = new PrintServer())
        {
            return printServer
                .GetPrintQueues() /// Get available printers
                .FirstOrDefault(p =>
                    p.QueueStatus != PrintQueueStatus.None && // Ensure the printer has no errors
                    !p.IsOffline && // Exclude offline printers
                    !p.IsHidden && // Exclude hidden printers
                    !p.Name.Contains("OneNote") && // Exclude Microsoft OneNote (not a real printer)
                    !p.Name.Contains("XPS") // Exclude XPS Document Writer (not a real printer)
                );
        }
    }

    /// <summary>
    /// Retrieves the "Microsoft Print to PDF" printer if available.
    /// </summary>
    /// <returns>The "Microsoft Print to PDF" <see cref="PrintQueue"/>, or <c>null</c> if not found.</returns>
    private PrintQueue? GetPdfPrinter()
    {
        using (PrintServer printServer = new PrintServer())
        {
            return printServer
                .GetPrintQueues() // Get available printers
                .FirstOrDefault(p =>
                    p.Name.Contains("Microsoft Print to PDF") // Find the "Microsoft Print to PDF" printer
                );
        }
    }
}

