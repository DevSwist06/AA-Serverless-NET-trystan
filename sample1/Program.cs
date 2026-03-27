
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
namespace sample1;

public class Personne
{
    public string Nom { get; set; }
    public int Age { get; set; }

    public Personne(string nom, int age)
    {
        Nom = nom;
        Age = age;
    }

    public string Hello(bool isLowercase)
    {
        string message = $"hello {Nom}, you are {Age}";
        if (isLowercase)
        {
            return message.ToLower();
        }
        else
        {
            return message.ToUpper();
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        string imagesDir = Path.Combine("images");
        string[] files = Directory.GetFiles(imagesDir);
        string outputDir = Path.Combine("images", "resized");
        Directory.CreateDirectory(outputDir);

        var sw = Stopwatch.StartNew();
        Parallel.ForEach(files, file =>
        {
            using (Image image = Image.Load(file))
            {
                image.Mutate(x => x.Resize(400, 300));
                string fileName = Path.GetFileNameWithoutExtension(file) + "-small" + Path.GetExtension(file);
                string outPath = Path.Combine(outputDir, fileName);
                image.Save(outPath);
                Console.WriteLine($"Image traitée : {outPath}");
            }
        });
        sw.Stop();
        Console.WriteLine($"Traitement parallèle terminé en {sw.ElapsedMilliseconds} ms");

        var personne = new Personne("Alice", 30);
        string json = JsonConvert.SerializeObject(personne, Formatting.Indented);
        Console.WriteLine(json);
    }
}
