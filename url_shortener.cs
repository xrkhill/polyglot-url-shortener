using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

class Shortener
{
    static void Main(string[] args) {
        Console.WriteLine("Hello, world!");

        var url = "http://www.google.com";

        var shortener = new Shortener();

        var result = shortener.Shorten(url);

        Console.WriteLine(url);
    }

    public string Shorten(string url)
    {
        var combined = url;

        System.Security.Cryptography.SHA1 sha1 = new SHA1CryptoServiceProvider();
        var digest = sha1.ComputeHash(Encoding.UTF8.GetBytes(combined));
        var integer = DigestToInteger(digest);

        var result = IntToBase36(integer);

        return result;
    }

    private BigInteger DigestToInteger(byte [] digest)
    {
        return new BigInteger(digest);
    }

    private const int Base = 36;
    private const string Charachters = "0123456789abcdefghijklmnopqrstuvwxyz";

    private string IntToBase36(BigInteger integer)
    {
        var result = "";

        while (integer > 0)
        {
            result = Charachters[integer % Base] + result;
            integer /= Base;
        }

        return result;
    }
}
