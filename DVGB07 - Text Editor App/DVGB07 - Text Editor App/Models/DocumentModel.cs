using System;
using System.ComponentModel;
using System.IO;

namespace DVGB07_Text_Editor_App.Models;

/// <summary>
/// Represents the model for a document in the text editor.
/// </summary>
public class DocumentModel : INotifyPropertyChanged
{
    private string _content;
    private string _filePath;
    private string _fileName;
    private bool _isModified;
    private (int Words, int charactersWithSpaces, int CharactersWithoutSpaces, int Rows) _textStats;

    /// <summary>
    /// Gets or sets the content of the document.
    /// </summary>
    public string Content
    {
        get => _content;
        set
        {
            if (_content != value)
            {
                _content = value;
                _textStats = GetTextStats(_content);
                _isModified = true;

                // Notify property changes
                OnPropertyChanged(nameof(Content));
                OnPropertyChanged(nameof(WordCount));
                OnPropertyChanged(nameof(CharacterWithSpacesCount));
                OnPropertyChanged(nameof(CharacterWithoutSpacesCount));
                OnPropertyChanged(nameof(RowCount));
                OnPropertyChanged(nameof(IsModified));
            }
        }
    }

    /// <summary>
    /// Gets the word count of the document.
    /// </summary>
    public int WordCount => _textStats.Words;

    /// <summary>
    /// Gets the character count of the document.
    /// </summary>
    public int CharacterWithSpacesCount => _textStats.charactersWithSpaces;


    /// <summary>
    /// Gets the character count of the document, excluding spaces.
    /// </summary>
    public int CharacterWithoutSpacesCount => _textStats.CharactersWithoutSpaces;

    /// <summary>
    /// Gets the row count of the document.
    /// </summary>
    public int RowCount => _textStats.Rows;

    /// <summary>
    /// Gets or sets the file path of the document.
    /// </summary>
    public string FilePath
    {
        get => _filePath;
        set
        {
            if (_filePath != value)
            {
                _filePath = value;
                FileName = Path.GetFileName(_filePath); // Extracts the file name from the full path
                OnPropertyChanged(nameof(FilePath));
                OnPropertyChanged(nameof(FileName));
            }
        }
    }

    /// <summary>
    /// Gets the file name of the document (extracted from the file path).
    /// </summary>
    public string FileName
    {
        get => _fileName;
        private set
        {
            if (_fileName != value)
            {
                _fileName = value;
                OnPropertyChanged(nameof(FileName));
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the document content has been modified.
    /// </summary>
    public bool IsModified
    {
        get => _isModified;
        private set
        {
            if (_isModified != value)
            {
                _isModified = value;
                OnPropertyChanged(nameof(IsModified));
            }
        }
    }

    /// <summary>
    /// Calculates text statistics, including the word count, character count with spaces, 
    /// character count without spaces, and row count for the given content.
    /// </summary>
    /// <param name="content">The content to analyze.</param>
    /// <returns>
    /// A tuple containing the following statistics:
    /// <list type="bullet">
    ///   <item>
    ///     <term>Words</term>
    ///     <description>The total number of words in the content.</description>
    ///   </item>
    ///   <item>
    ///     <term>charactersWithSpaces</term>
    ///     <description>The total number of characters, including spaces.</description>
    ///   </item>
    ///   <item>
    ///     <term>CharactersWithoutSpaces</term>
    ///     <description>The total number of characters, excluding spaces.</description>
    ///   </item>
    ///   <item>
    ///     <term>Rows</term>
    ///     <description>The total number of rows (lines) in the content, based on newline characters.</description>
    ///   </item>
    /// </list>
    /// </returns>
    private static (int Words, int charactersWithSpaces, int CharactersWithoutSpaces, int Rows) GetTextStats(string content)
    {
        int words = 0;
        int charactersWithSpaces = 0;
        int charactersWithoutSpaces = 0;
        int rows = 1; // Initialize rows to 1 since an empty string counts as a single row
        bool inWord = false;

        if (string.IsNullOrEmpty(content))
            return (words, charactersWithSpaces, charactersWithoutSpaces, rows);

        foreach (char c in content)
        {
            // Count characters excluding newlines (both \r and \n)
            if (c != '\r' && c != '\n')
            {
                charactersWithSpaces++;
                if (!char.IsWhiteSpace(c))
                    charactersWithoutSpaces++;
            }

            // Count rows (newlines, we only need to count \n)
            if (c == '\n')
            {
                rows++;
            }

            // Count words by checking transitions from non-whitespace to non-whitespace
            if (char.IsWhiteSpace(c))
            {
                inWord = false;
            }
            else if (!inWord)
            {
                inWord = true;
                words++;
            }
        }

        return (words, charactersWithSpaces, charactersWithoutSpaces, rows);
    }


    /// <summary>
    /// Marks the document as saved by resetting the modified state.
    /// </summary>
    public void MarkAsSaved()
    {
        IsModified = false;
    }

    /// <summary>
    /// Event triggered when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Notifies listeners that a property value has changed.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
