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
        var personne = new Personne("Alice", 30);
        Console.WriteLine(personne.Hello(true));
        Console.WriteLine(personne.Hello(false));
    }
}
