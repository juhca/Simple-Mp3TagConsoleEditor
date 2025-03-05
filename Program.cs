using System.Reflection;
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
        TagLib.Tag tags = file.Tag;
        TagLib.Properties properties = file.Properties;

        // reflection
        PropertyInfo[] tagProperties = tags.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        PropertyInfo[] propProperties = properties.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        
        DisplayProperties(tagProperties, tags);
        DisplayProperties(propProperties, properties);
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
        Console.WriteLine($"Edit metadata {file.Tag.Title}");
    }
}