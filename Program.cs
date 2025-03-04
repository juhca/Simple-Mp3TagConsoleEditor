using System.Reflection;

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
        foreach (var tag in tagProperties)
        {
            try
            {
                var name = tag.Name;
                var value = tag.GetValue(tags, null);
                if (value == null) continue;
                
                if (name.Contains("StartTag") || name.Contains("EndTag") || name.Contains("Joined"))
                {
                    continue;
                }

                if (value is Array array)
                {
                    if (array.Length == 0)
                    {
                        Console.WriteLine($"{tag.Name}: /");
                    }
                    else if (array.Length == 1)
                    {
                        Console.WriteLine($"{tag.Name}: {array.GetValue(0) ?? '/'}");
                    }
                    else
                    {
                        Console.WriteLine($"{tag.Name}: [{string.Join(", ", array.Cast<object>())}]");
                    }
                }
                else
                {
                    Console.WriteLine($"{tag.Name}: {value}");
                }
            }
            catch (Exception ex)
            {
                continue;
            }
        }
        
        
    }

    private static void EditMetadata(TagLib.File file)
    {
        Console.WriteLine($"Edit metadata {file.Tag.Title}");
    }
}