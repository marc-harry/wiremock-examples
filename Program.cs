using System;
using System.Linq;
using System.Threading.Tasks;
using WireMock.Net.OpenApiParser;
using WireMock.Server;

namespace wiremock
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = WireMockServer.StartWithAdminInterface(3333);

            var mappingModels = new WireMockOpenApiParser().FromFile("./test.yaml", out var diagnostic).ToArray();
            Console.WriteLine(diagnostic);

            if (!args.Contains("nomappings"))
            {
                server.WithMapping(mappingModels);

                Console.WriteLine($"Added {mappingModels.Length} mappings");
                foreach (var mapping in mappingModels)
                {
                    Console.WriteLine(mapping.Request.Path);
                }
            }
            Console.WriteLine($"Server running at: {string.Join(',', server.Urls)}");

            Console.ReadLine();
        }

        // async static Task StandaloneServer()
        // {
        //     // see source code for all the possible properties
        //     var settings = new FluentMockServerSettings
        //     {
        //         AllowPartialMapping = true,
        //         StartAdminInterface = true
        //     };
        //     StandAloneApp.Start(settings);

        //     await Task.Delay(-1);
        // }
    }
}
