using CommandLine;
using LanguageSdk.Templates.Core;

namespace BackseatC;

public static class Program
{
    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<DriverSettings>(args)
            .WithParsed(options =>
            {
                var driver = Driver.Create(options);

                _ = driver.Compile();
            })
            .WithNotParsed(errors =>
            {
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ToString());
                }
            });
    }
}