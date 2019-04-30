using System;
using System.IO;

namespace HtmlPatch
{
    class Program
    {
        static void Main(string[] args)
        {
            var fooCurrent = Directory.GetCurrentDirectory();
            var fooProcessHome = Path.Combine(fooCurrent, "..", "..", "..", "..", "..", "9LabBook");
            Console.WriteLine(fooProcessHome);
            //D:\Vulcan\GitHub\XamarinLOBForms\HtmlPatch\HtmlPatch\bin\Debug\netcoreapp2.0\..\..\..\..\..\9LabBook
            var fooAllHtmls = Directory.GetFiles(fooProcessHome, "*.html");
            foreach (var item in fooAllHtmls)
            {
                var fooContent = File.ReadAllText(item);
                var fooContentNew = fooContent.Replace("file:///d%3A/Vulcan/GitHub/XamarinLOBForms/9LabBook/", "");
                if (fooContent != fooContentNew)
                {
                    Console.WriteLine($"{item}");
                }
                File.WriteAllText(item, fooContentNew);
            }
            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();

        }
    }
}
