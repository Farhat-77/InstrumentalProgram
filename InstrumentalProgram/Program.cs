using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace InstrumentalProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileInputName = @"OneExeProgram\Libraries\AutoMapper 1.0 RTW\AutoMapper.dll";
            var assembly = File.ReadAllBytes(fileInputName);

            var fileOutputName = @"OneExeProgram\Libraries\AutoMapper 1.0 RTW\AutoMapper.dll.deflated";
            using (var file = File.Open(fileOutputName, FileMode.Create))
            using (var stream = new DeflateStream(file, CompressionMode.Compress))
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(assembly);
            }
        }
        private static Assembly AppDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("AutoMapper"))
            {
                Console.WriteLine("Resolving assembly: {0}", args.Name);

                // Загрузка запакованной сборки из ресурсов, ее распаковка и подстановка
                using (var resource = new MemoryStream(System.Resources.AutoMapper_dll))
                using (var deflated = new DeflateStream(resource, CompressionMode.Decompress))
                using (var reader = new BinaryReader(deflated))
                {
                    var one_megabyte = 1024 * 1024;
                    var buffer = reader.ReadBytes(one_megabyte);
                    return Assembly.Load(buffer);
                }
            }

            return null;
        }
    }
}
