﻿using System.Reflection;
using TagLib;

namespace Mp3TagConsoleEditor;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: Mp3TagConsoleEditor <input-file> <output-file>");
            return;
        }
            
        try
        {
            var tfile = TagLib.File.Create(args[0]);

            if (tfile.Properties.MediaTypes != MediaTypes.Audio)
            {
                Console.WriteLine("Unsupported media type");
                return;
            }
            
            string? mode;
            do
            {
                Console.WriteLine("What would you like to do?");
                Console.Write("0 - Show metadata, 1 - Edit metadata or 2 - exit: ");
                mode = Console.ReadLine();
            } while (string.IsNullOrEmpty(mode));

            switch (mode)
            {
                case "0":
                    ShowMetadata(tfile);
                    break;
                case "1":
                    EditMetadata(tfile);
                    break;
                default:
                    Console.WriteLine("Thank you for using this program.");
                    return;
            }
                
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine("File not found: "+ ex.Message);
            return;
        }
    }

    private static void ShowMetadata(TagLib.File file)
    {
        Console.WriteLine($"\nShowing metadata: {file.Tag.Title}\n");

        // reflection
        PropertyInfo[] tagProperties = file.Tag.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        PropertyInfo[] propProperties = file.Properties.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        
        DisplayProperties(tagProperties, file.Tag);
        DisplayProperties(propProperties, file.Properties);
    }

    private static void DisplayProperties(PropertyInfo[] properties, object tagLibObject)
    {
        foreach (var property in properties)
        {
            try
            {
                var name = property.Name;
                var value = property.GetValue(tagLibObject, null);
                if (value == null) continue;

                if (name.Contains("StartTag") || name.Contains("EndTag") || name.Contains("Joined"))
                {
                    continue;
                }

                if (value is Array array)
                {
                    switch (array.Length)
                    {
                        case 0:
                            Console.WriteLine($"{property.Name}: /");
                            break;
                        case 1:
                            Console.WriteLine($"{property.Name}: {array.GetValue(0) ?? '/'}");
                            break;
                        default:
                            Console.WriteLine($"{property.Name}: [{string.Join(", ", array.Cast<object>())}]");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine($"{property.Name}: {value}");
                }
            }
            catch (Exception ex)
            {
            }
        }
        
        Console.WriteLine("\n=========================\n");
    }

    private static void EditMetadata(TagLib.File file)
    {
        while (true)
        {
            Console.WriteLine("\nSelect a field to edit:");
            Console.WriteLine("1. Title");
            Console.WriteLine("2. Artists");
            Console.WriteLine("3. Album");
            Console.WriteLine("4. Year");
            Console.WriteLine("5. Track Number");
            Console.WriteLine("6. Genres");
            Console.WriteLine("7. Comment");
            Console.WriteLine("8. Save and Exit");
            Console.WriteLine("9. Exit without saving");
            
            Console.Write("Enter your choice: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter title: ");
                    file.Tag.Title = Console.ReadLine()?.Trim();
                    break;
                case "2":
                    Console.Write("Enter artists (separated by comma): ");
                    var artists = Console.ReadLine();
                    file.Tag.AlbumArtists = string.IsNullOrEmpty(artists) ? 
                        [] : artists.Split(',').Select(x => x.Trim()).ToArray();
                    break;
                case "3":
                    Console.Write("Enter album: ");
                    file.Tag.Album = Console.ReadLine()?.Trim();
                    break;
                case "4":
                    Console.Write("Enter year: ");
                    if (uint.TryParse(Console.ReadLine(), out var parsedYear))
                    {
                        file.Tag.Year = parsedYear;
                    }
                    else
                    {
                        Console.WriteLine("Invalid year. Must be a positive number.");
                    }
                    break;
                case "5":
                    Console.Write("Enter track number: ");
                    if (uint.TryParse(Console.ReadLine(), out var trackNumber))
                    {
                        file.Tag.Track = trackNumber;
                    }
                    else
                    {
                        Console.WriteLine("Invalid track number. Must be a positive number.");
                    }
                    break;
                case "6":
                    Console.Write("Enter genres: (separated by comma): ");
                    var genres = Console.ReadLine();
                    file.Tag.Genres = string.IsNullOrEmpty(genres) ? 
                        [] : genres.Split(',').Select(x => x.Trim()).ToArray();
                    break;
                case "7":
                    Console.Write("Enter comment: ");
                    file.Tag.Comment = Console.ReadLine()?.Trim();
                    break;
                case "8":
                    Console.Write("Saving and exiting...");
                    file.Save();
                    return;
                case "9":
                    Console.Write("Exiting without saving... ");
                    return;
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
    }
}