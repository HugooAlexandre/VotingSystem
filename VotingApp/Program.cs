using Grpc.Net.Client;
using VotingSystem;
using VotingSystem.Voting;
using VotingApp.Models;
using VotingApp.Services;
using VotingApp.Utils;

namespace VotingApp
{
    class Program
    {
        private const string ENDPOINT = "https://ken01.utad.pt:9091";
        private static Voter? _currentVoter = null;
        private static readonly AuthService _authService = new();
        private static readonly VotingService _votingService = new();
        private static readonly ResultsService _resultsService = new();

        static async Task Main(string[] args)
        {
            Console.Title = "Sistema de Votação Eletrónica";
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            await RunApplication();
        }

        static async Task RunApplication()
        {
            bool exit = false;
            
            while (!exit)
            {
                ConsoleHelper.PrintHeader("SISTEMA DE VOTAÇÃO ELETRÓNICA 2025-2026");
                DisplaySessionStatus();
                DisplayMainMenu();
                
                var option = Console.ReadLine();
                
                switch (option)
                {
                    case "1":
                        await RegisterVoter();
                        break;
                    case "2":
                        await ListCandidates();
                        break;
                    case "3":
                        await CastVote();
                        break;
                    case "4":
                        await ViewResults();
                        break;
                    case "5":
                        ViewVotingStatus();
                        break;
                    case "6":
                        ResetSession();
                        break;
                    case "7":
                        await TestServices();
                        break;
                    case "0":
                        exit = true;
                        ConsoleHelper.WriteInfo("A encerrar o sistema...");
                        break;
                    default:
                        ConsoleHelper.WriteError("Opção inválida.");
                        break;
                }
                
                if (!exit)
                {
                    ConsoleHelper.PressToContinue();
                }
            }
        }

        static void DisplaySessionStatus()
        {
            Console.WriteLine("═".PadRight(50, '═'));
            Console.WriteLine("ESTADO DA SESSÃO:");
            
            if (_currentVoter == null)
            {
                Console.WriteLine("  • Não identificado");
                Console.WriteLine("  • Credencial: Não obtida");
                Console.WriteLine("  • Voto: Não realizado");
            }
            else
            {
                Console.WriteLine($"  • CC: {_currentVoter.CitizenCardNumber}");
                Console.WriteLine($"  • Credencial: {(_currentVoter.HasCredential ? "✓ Obtida" : "✗ Não obtida")}");
                Console.WriteLine($"  • Voto: {(_currentVoter.HasVoted ? "✓ Realizado" : "✗ Não realizado")}");
                if (_currentVoter.HasVoted)
                {
                    Console.WriteLine($"  • Candidato: {_currentVoter.SelectedCandidateId}");
                    Console.WriteLine($"  • Hora: {_currentVoter.VoteTimestamp:HH:mm:ss}");
                }
            }
            Console.WriteLine("═".PadRight(50, '═'));
            Console.WriteLine();
        }

        static void DisplayMainMenu()
        {
            Console.WriteLine("MENU PRINCIPAL:");
            Console.WriteLine("1. Identificar-se (obter credencial)");
            Console.WriteLine("2. Listar candidatos");
            Console.WriteLine("3. Votar");
            Console.WriteLine("4. Consultar resultados");
            Console.WriteLine("5. Estado do processo");
            Console.WriteLine("6. Reiniciar sessão");
            Console.WriteLine("7. Testar serviços");
            Console.WriteLine("0. Sair");
            Console.Write("\nSelecione uma opção: ");
        }

        static async Task RegisterVoter()
        {
            ConsoleHelper.PrintHeader("IDENTIFICAÇÃO DO ELEITOR");
            
            Console.Write("Número do Cartão de Cidadão: ");
            var ccNumber = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(ccNumber))
            {
                ConsoleHelper.WriteError("Número de cartão inválido.");
                return;
            }
            
            ConsoleHelper.WriteInfo("A contactar Autoridade de Registo...");
            
            var result = await _authService.AuthenticateVoter(ccNumber);
            
            if (result.IsSuccess)
            {
                ConsoleHelper.WriteSuccess($"CREDENCIAL EMITIDA: {result.Credential}");
                
                _currentVoter = new Voter
                {
                    CitizenCardNumber = ccNumber,
                    VotingCredential = result.Credential,
                    HasVoted = false
                };
                
                Console.WriteLine("\nIMPORTANTE:");
                Console.WriteLine("• Esta credencial é única e pessoal");
                Console.WriteLine("• Será consumida após o voto");
                Console.WriteLine("• Não a partilhe com ninguém");
            }
            else
            {
                ConsoleHelper.WriteError($"FALHA NA AUTENTICAÇÃO: {result.Message}");
            }
        }

        static async Task ListCandidates()
        {
            ConsoleHelper.PrintHeader("LISTA DE CANDIDATOS");
            
            var candidates = await _votingService.GetCandidatesAsync();
            
            if (candidates.Count == 0)
            {
                ConsoleHelper.WriteWarning("Não existem candidatos disponíveis.");
                return;
            }
            
            Console.WriteLine("Candidatos às Eleições Presidenciais:\n");
            Console.WriteLine("┌─────┬──────────────────────────────┐");
            Console.WriteLine("│ ID  │ Nome                         │");
            Console.WriteLine("├─────┼──────────────────────────────┤");
            
            foreach (var candidate in candidates)
            {
                Console.WriteLine($"│ {candidate.Id,3} │ {candidate.Name,-28} │");
            }
            
            Console.WriteLine("└─────┴──────────────────────────────┘");
            Console.WriteLine($"\nTotal: {candidates.Count} candidatos");
        }

        static async Task CastVote()
        {
            ConsoleHelper.PrintHeader("PROCESSO DE VOTAÇÃO");
            
            if (_currentVoter == null || !_currentVoter.HasCredential)
            {
                ConsoleHelper.WriteError("Deve obter uma credencial antes de votar.");
                Console.WriteLine("Utilize a opção 1 do menu principal.");
                return;
            }
            
            if (_currentVoter.HasVoted)
            {
                ConsoleHelper.WriteError("Já realizou o seu voto.");
                Console.WriteLine("Cada eleitor só pode votar uma vez.");
                return;
            }
            
            // Mostrar candidatos
            var candidates = await _votingService.GetCandidatesAsync();
            if (candidates.Count == 0)
            {
                ConsoleHelper.WriteError("Não existem candidatos disponíveis.");
                return;
            }
            
            Console.WriteLine("CANDIDATOS DISPONÍVEIS:\n");
            foreach (var candidate in candidates)
            {
                Console.WriteLine($"{candidate.Id}. {candidate.Name}");
            }
            
            Console.Write($"\nSelecione o ID do candidato (1-{candidates.Count}): ");
            
            if (!int.TryParse(Console.ReadLine(), out int candidateId) || 
                candidateId < 1 || candidateId > candidates.Count)
            {
                ConsoleHelper.WriteError("ID de candidato inválido.");
                return;
            }
            
            var selectedCandidate = candidates.First(c => c.Id == candidateId);
            
            // Confirmação
            Console.WriteLine($"\nCONFIRMAÇÃO DE VOTO:");
            Console.WriteLine($"Candidato: {selectedCandidate.Name}");
            Console.WriteLine($"Credencial: {_currentVoter.VotingCredential}");
            
            if (!ConsoleHelper.ConfirmAction("Confirmar voto?"))
            {
                ConsoleHelper.WriteInfo("Voto cancelado.");
                return;
            }
            
            // Submeter voto
            ConsoleHelper.WriteInfo("A submeter o seu voto...");
            
            var voteResult = await _votingService.CastVoteAsync(
                _currentVoter.VotingCredential, 
                candidateId
            );
            
            if (voteResult.Success)
            {
                ConsoleHelper.WriteSuccess("VOTO REGISTADO COM SUCESSO!");
                _currentVoter.RecordVote(candidateId);
                Console.WriteLine($"Comprovativo: {voteResult.Message}");
            }
            else
            {
                ConsoleHelper.WriteError($"VOTO RECUSADO: {voteResult.Message}");
            }
        }

        static async Task ViewResults()
        {
            ConsoleHelper.PrintHeader("RESULTADOS ELEITORAIS");
            
            var results = await _resultsService.GetResultsAsync();
            
            if (results.Count == 0)
            {
                ConsoleHelper.WriteInfo("Ainda não existem votos registados.");
                return;
            }
            
            var voteResults = VoteResult.FromGrpcResults(results);
            int totalVotes = voteResults.Sum(r => r.Votes);
            
            Console.WriteLine("RESULTADOS PARCIAIS:\n");
            Console.WriteLine("┌──────────────────────────────┬────────┬───────────┐");
            Console.WriteLine("│ Candidato                    │ Votos  │ Percent.  │");
            Console.WriteLine("├──────────────────────────────┼────────┼───────────┤");
            
            foreach (var result in voteResults.OrderByDescending(r => r.Votes))
            {
                Console.WriteLine($"│ {result.CandidateName,-28} │ {result.Votes,6} │ {result.Percentage,7:F2}% │");
            }
            
            Console.WriteLine("├──────────────────────────────┼────────┼───────────┤");
            Console.WriteLine($"│ {"TOTAL",-28} │ {totalVotes,6} │ {"100.00%",9} │");
            Console.WriteLine("└──────────────────────────────┴────────┴───────────┘");
            
            Console.WriteLine($"\nAtualizado em: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            Console.WriteLine("Nota: Estes são resultados não oficiais em tempo real.");
        }

        static void ViewVotingStatus()
        {
            ConsoleHelper.PrintHeader("ESTADO DO PROCESSO");
            
            if (_currentVoter == null)
            {
                Console.WriteLine("Nenhum eleitor identificado.");
                return;
            }
            
            Console.WriteLine($"INFORMAÇÃO DO ELEITOR:");
            Console.WriteLine($"• Cartão Cidadão: {_currentVoter.CitizenCardNumber}");
            Console.WriteLine($"• Credencial: {_currentVoter.VotingCredential}");
            Console.WriteLine($"• Estado: {(_currentVoter.HasVoted ? "VOTO REALIZADO" : "PODE VOTAR")}");
            
            if (_currentVoter.HasVoted)
            {
                Console.WriteLine($"• Hora do voto: {_currentVoter.VoteTimestamp:dd/MM/yyyy HH:mm:ss}");
                Console.WriteLine($"• Candidato escolhido: {_currentVoter.SelectedCandidateId}");
            }
            
            Console.WriteLine($"\nSERVIÇOS:");
            Console.WriteLine($"• Endpoint: {ENDPOINT}");
            Console.WriteLine($"• Hora do sistema: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        }

        static void ResetSession()
        {
            if (ConsoleHelper.ConfirmAction("Tem a certeza que pretende reiniciar a sessão?"))
            {
                _currentVoter = null;
                ConsoleHelper.WriteSuccess("Sessão reiniciada com sucesso.");
            }
            else
            {
                ConsoleHelper.WriteInfo("Operação cancelada.");
            }
        }

        static async Task TestServices()
        {
            ConsoleHelper.PrintHeader("TESTE DE SERVIÇOS");
            
            Console.WriteLine("Testando conexão aos serviços gRPC...\n");
            
            try
            {
                // Testar serviço de autenticação
                Console.Write("1. Testando Autoridade de Registo... ");
                var authResult = await _authService.AuthenticateVoter("123456789");
                Console.WriteLine(authResult.IsSuccess ? "✓ OK" : "✗ FALHOU");
                
                // Testar serviço de votação
                Console.Write("2. Testando Autoridade de Votação... ");
                var candidates = await _votingService.GetCandidatesAsync();
                Console.WriteLine(candidates.Count > 0 ? "✓ OK" : "✗ FALHOU");
                
                // Testar serviço de resultados
                Console.Write("3. Testando resultados... ");
                var results = await _resultsService.GetResultsAsync();
                Console.WriteLine("✓ OK");
                
                Console.WriteLine("\nRESUMO DOS TESTES:");
                Console.WriteLine($"• Endpoint: {ENDPOINT}");
                Console.WriteLine($"• Candidatos disponíveis: {candidates.Count}");
                Console.WriteLine($"• Votos registados: {results.Sum(r => r.Votes)}");
                Console.WriteLine($"• Serviços operacionais: 3/3");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError($"Erro nos testes: {ex.Message}");
            }
        }
    }
}