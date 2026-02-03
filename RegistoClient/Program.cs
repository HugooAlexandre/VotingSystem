using Grpc.Net.Client;
using VotingSystem;

namespace RegistoClient
{
    class Program
    {
        private const string ENDPOINT = "https://ken01.utad.pt:9091";
        
        static async Task Main(string[] args)
        {
            Console.Title = "Cliente AR - Autoridade de Registo";
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("    AUTORIDADE DE REGISTO (AR) - CLIENTE");
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine($"Endpoint: {ENDPOINT}");
            Console.WriteLine();
            
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = 
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            
            using var channel = GrpcChannel.ForAddress(ENDPOINT, 
                new GrpcChannelOptions { HttpHandler = handler });
            
            var client = new VoterRegistrationService.VoterRegistrationServiceClient(channel);
            
            await RunClient(client);
        }
        
        static async Task RunClient(VoterRegistrationService.VoterRegistrationServiceClient client)
        {
            int total = 0, valid = 0, invalid = 0;
            
            Console.WriteLine("TESTE DE EMISSÃO DE CREDENCIAIS");
            Console.WriteLine("(Introduza 'sair' para terminar)");
            Console.WriteLine();
            
            while (true)
            {
                Console.Write("N.º Cartão Cidadão: ");
                var cc = Console.ReadLine();
                
                if (string.IsNullOrWhiteSpace(cc) || cc.ToLower() == "sair")
                    break;
                
                total++;
                
                try
                {
                    var reply = await client.IssueVotingCredentialAsync(
                        new VoterRequest { CitizenCardNumber = cc });
                    
                    if (reply.IsEligible)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"✓ CREDENCIAL: {reply.VotingCredential}");
                        valid++;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"✗ {reply.VotingCredential}");
                        invalid++;
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"ERRO: {ex.Message}");
                    invalid++;
                }
                finally
                {
                    Console.ResetColor();
                }
                
                Console.WriteLine($"Estatística: Válidas={valid}, Inválidas={invalid}, Total={total}");
                Console.WriteLine();
            }
            
            Console.WriteLine("\n═══════════════════════════════════════════════");
            Console.WriteLine("RESUMO FINAL:");
            Console.WriteLine($"• Pedidos: {total}");
            Console.WriteLine($"• Credenciais válidas: {valid} ({CalculatePercentage(valid, total)}%)");
            Console.WriteLine($"• Credenciais inválidas: {invalid} ({CalculatePercentage(invalid, total)}%)");
            Console.WriteLine("═══════════════════════════════════════════════");
        }
        
        static double CalculatePercentage(int value, int total)
        {
            return total > 0 ? Math.Round((value * 100.0) / total, 1) : 0;
        }
    }
}