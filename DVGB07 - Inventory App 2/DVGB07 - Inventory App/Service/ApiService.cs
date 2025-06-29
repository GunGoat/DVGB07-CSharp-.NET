using DVGB07_Inventory_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DVGB07_Inventory_App.Service;

/// <summary>
/// Provides API services for retrieving and updating product inventory.
/// </summary>
public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private const string ApiUrl = "https://hex.cse.kau.se/~jonavest/csharp-api";

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiService"/> class.
    /// </summary>
    public ApiService()
    {
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// Retrieves a list of products from the API.
    /// </summary>
    /// <returns>A tuple containing a list of products and an error message if applicable.</returns>
    public async Task<(List<Product> Products, string ErrorMessage)> GetProductsAsync()
    {
        try
        {
            var response = await _httpClient.GetStringAsync(ApiUrl);
            return ParseResponse(response);
        }
        catch (InvalidOperationException ex)
        {
            return (new List<Product>(), $"Data parsing error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            return (new List<Product>(), $"HTTP error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return (new List<Product>(), $"Unexpected error: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates the stock of a specified product.
    /// </summary>
    /// <param name="id">The product ID.</param>
    /// <param name="stock">The new stock value.</param>
    /// <returns>A tuple containing an updated list of products and an error message if applicable.</returns>
    public async Task<(List<Product> Products, string ErrorMessage)> UpdateStockAsync(int id, int stock)
    {
        try
        {
            string requestUrl = $"{ApiUrl}/?action=update&id={id}&stock={stock}";
            var response = await _httpClient.GetStringAsync(requestUrl);
            return ParseResponse(response);
        }
        catch (InvalidOperationException ex)
        {
            return (new List<Product>(), $"Data parsing error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            return (new List<Product>(), $"HTTP error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return (new List<Product>(), $"Unexpected error: {ex.Message}");
        }
    }

    /// <summary>
    /// Parses the XML response from the API.
    /// </summary>
    /// <param name="xml">The XML response string.</param>
    /// <returns>A tuple containing a list of products and an error message if applicable.</returns>
    private (List<Product> Products, string ErrorMessage) ParseResponse(string xml)
    {
        var xDoc = XDocument.Parse(xml);

        var errorElement = xDoc.Descendants("error").FirstOrDefault();
        if (errorElement != null)
        {
            return (new List<Product>(), errorElement.Value);
        }

        return (ParseProducts(xDoc), null);
    }

    /// <summary>
    /// Extracts product information from the XML document.
    /// </summary>
    /// <param name="xDoc">The XML document containing product data.</param>
    /// <returns>A list of products.</returns>
    private List<Product> ParseProducts(XDocument xDoc)
    {
        var products = new List<Product>();

        foreach (var book in xDoc.Descendants("book"))
        {
            products.Add(new Book
            {
                Id = GetRequiredInt(book, "id"),
                Name = GetRequiredString(book, "name"),
                Price = GetRequiredDecimal(book, "price"),
                Stock = GetRequiredInt(book, "stock"),
                Genre = book.Element("genre")?.Value,
                Format = book.Element("format")?.Value,
                Language = book.Element("language")?.Value
            });
        }

        foreach (var game in xDoc.Descendants("game"))
        {
            products.Add(new Game
            {
                Id = GetRequiredInt(game, "id"),
                Name = GetRequiredString(game, "name"),
                Price = GetRequiredDecimal(game, "price"),
                Stock = GetRequiredInt(game, "stock"),
                Platform = game.Element("platform")?.Value
            });
        }

        foreach (var movie in xDoc.Descendants("movie"))
        {
            products.Add(new Movie
            {
                Id = GetRequiredInt(movie, "id"),
                Name = GetRequiredString(movie, "name"),
                Price = GetRequiredDecimal(movie, "price"),
                Stock = GetRequiredInt(movie, "stock"),
                Format = movie.Element("format")?.Value,
                Playtime = int.TryParse(movie.Element("playtime")?.Value, out int playtime) ? playtime : (int?)null
            });
        }

        return products;
    }

    /// <summary>
    /// Retrieves a required integer value from an XML element.
    /// </summary>
    /// <param name="element">The XML element.</param>
    /// <param name="name">The name of the XML element to retrieve.</param>
    /// <returns>The integer value.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the value is missing or invalid.</exception>
    private int GetRequiredInt(XElement element, string name)
    {
        var value = element.Element(name)?.Value;
        if (int.TryParse(value, out int result))
        {
            return result;
        }
        throw new InvalidOperationException($"Missing or invalid '{name}' value.");
    }

    /// <summary>
    /// Retrieves a required decimal value from an XML element.
    /// </summary>
    /// <param name="element">The XML element.</param>
    /// <param name="name">The name of the XML element to retrieve.</param>
    /// <returns>The decimal value.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the value is missing or invalid.</exception>
    private decimal GetRequiredDecimal(XElement element, string name)
    {
        var value = element.Element(name)?.Value;
        if (decimal.TryParse(value, out decimal result))
        {
            return result;
        }
        throw new InvalidOperationException($"Missing or invalid '{name}' value.");
    }

    /// <summary>
    /// Retrieves a required string value from an XML element.
    /// </summary>
    /// <param name="element">The XML element.</param>
    /// <param name="name">The name of the XML element to retrieve.</param>
    /// <returns>The string value.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the value is missing or empty.</exception>
    private string GetRequiredString(XElement element, string name)
    {
        var value = element.Element(name)?.Value;
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value;
        }
        throw new InvalidOperationException($"Missing or empty '{name}' value.");
    }
}
