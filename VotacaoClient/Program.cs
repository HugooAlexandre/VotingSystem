using Grpc.Net.Client;
using VotingSystem.Voting;

namespace VotacaoClient
{
    class Program
    {
        private const string ENDPOINT = "https://ken01.utad.pt:9091";
        
        static async Task Main(string[] args)
        {
            Console.Title = "Cliente AV - Autoridade de Votação";
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("    AUTORIDADE DE VOTAÇÃO (AV) - CLIENTE");
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine($"Endpoint: {ENDPOINT}");
            Console.WriteLine();
            
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = 
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            
            using var channel = GrpcChannel.ForAddress(ENDPOINT, 
                new GrpcChannelOptions { HttpHandler = handler });
            
            var client = new VotingService.VotingServiceClient(channel);
            
            await RunClient(client);
        }
        
        static async Task RunClient(VotingService.VotingServiceClient client)
        {
            while (true)
            {
                Console.WriteLine("MENU:");
                Console.WriteLine("1. Listar candidatos");
                Console.WriteLine("2. Submeter voto");
                Console.WriteLine("3. Consultar resultados");
                Console.WriteLine("4. Estado do sistema");
                Console.WriteLine("0. Sair");
                Console.Write("\nOpção: ");
                
                var option = Console.ReadLine();
                
                switch (option)
                {
                    case "1":
                        await ListCandidates(client);
                        break;
                    case "2":
                        await SubmitVote(client);
                        break;
                    case "3":
                        await ShowResults(client);
                        break;
                    case "4":
                        ShowSystemStatus(client);
                        break;
                    case "0":
                        Console.WriteLine("A encerrar cliente...");
                        return;
                    default:
                        Console.WriteLine("Opção inválida.");
                        break;
                }
                
                Console.WriteLine("\nPressione Enter para continuar...");
                Console.ReadLine();
                Console.Clear();
            }
        }
        
        static async Task ListCandidates(VotingService.VotingServiceClient client)
        {
            Console.WriteLine("\n── LISTA DE CANDIDATOS ──");
            
            try
            {
                var reply = await client.GetCandidatesAsync(new GetCandidatesRequest());
                
                if (reply.Candidates.Count == 0)
                {
                    Console.WriteLine("Não há candidatos disponíveis.");
                    return;
                }
                
                Console.WriteLine("ID | Nome");
                Console.WriteLine("───┼─────────────────────");
                
                foreach (var candidate in reply.Candidates)
                {
                    Console.WriteLine($"{candidate.Id,2} | {candidate.Name}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }
        
        static async Task SubmitVote(VotingService.VotingServiceClient client)
        {
            Console.WriteLine("\n── SUBMETER VOTO ──");
            
            Console.Write("Credencial de voto: ");
            var credential = Console.ReadLine();
            
            Console.Write("ID do candidato: ");
            if (!int.TryParse(Console.ReadLine(), out int candidateId))
            {
                Console.WriteLine("ID inválido.");
                return;
            }
            
            try
            {
                var reply = await client.VoteAsync(new VoteRequest
                {
                    VotingCredential = credential ?? "",
                    CandidateId = candidateId
                });
                
                if (reply.Success)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"✓ VOTO ACEITE: {reply.Message}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"✗ VOTO RECUSADO: {reply.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
            finally
            {
                Console.ResetColor();
            }
        }
        
        static async Task ShowResults(VotingService.VotingServiceClient client)
        {
            Console.WriteLine("\n── RESULTADOS ──");
            
            try
            {
                var reply = await client.GetResultsAsync(new GetResultsRequest());
                
                if (reply.Results.Count == 0)
                {
                    Console.WriteLine("Não há votos registados.");
                    return;
                }
                
                int total = reply.Results.Sum(r => r.Votes);
                
                Console.WriteLine("Candidato          | Votos   | %");
                Console.WriteLine("───────────────────┼─────────┼──────");
                
                foreach (var result in reply.Results.OrderByDescending(r => r.Votes))
                {
                    double percent = total > 0 ? (result.Votes * 100.0) / total : 0;
                    Console.WriteLine($"{result.Name,-18} | {result.Votes,7} | {percent,5:F1}%");
                }
                
                Console.WriteLine("───────────────────┼─────────┼──────");
                Console.WriteLine($"{"TOTAL",-18} | {total,7} | 100.0%");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }
        
        static void ShowSystemStatus(VotingService.VotingServiceClient client)
        {
            Console.WriteLine("\n── ESTADO DO SISTEMA ──");
            Console.WriteLine($"Endpoint: {ENDPOINT}");
            Console.WriteLine($"Data/Hora: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            Console.WriteLine($"Protocolo: gRPC com TLS");
            Console.WriteLine($"Serviço: VotingService");
        }
    }
}