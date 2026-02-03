using Grpc.Net.Client;
using VotingSystem.Voting;

namespace VotingApp.Services
{
    public class ResultsService
    {
        private readonly VotingServiceClient _client;
        
        public ResultsService()
        {
            var handler = Config.GetHttpClientHandler();
            var channel = GrpcChannel.ForAddress(Config.GetEndpoint(),
                new GrpcChannelOptions { HttpHandler = handler });
            _client = new VotingServiceClient(channel);
        }
        
        public async Task<List<CandidateResult>> GetResultsAsync()
        {
            try
            {
                var response = await _client.GetResultsAsync(new GetResultsRequest());
                return response.Results.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter resultados: {ex.Message}");
                return new List<CandidateResult>();
            }
        }
        
        public async Task<string> GetResultsSummaryAsync()
        {
            var results = await GetResultsAsync();
            if (results.Count == 0)
                return "Não há resultados disponíveis.";
            
            int totalVotes = results.Sum(r => r.Votes);
            var summary = "RESULTADOS ELEITORAIS:\n";
            summary += "----------------------------\n";
            
            foreach (var result in results.OrderByDescending(r => r.Votes))
            {
                double percentage = totalVotes > 0 ? (result.Votes * 100.0) / totalVotes : 0;
                summary += $"{result.Name}: {result.Votes} votos ({percentage:F2}%)\n";
            }
            
            summary += "----------------------------\n";
            summary += $"Total de votos: {totalVotes}\n";
            summary += $"Atualizado em: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            
            return summary;
        }
    }
}