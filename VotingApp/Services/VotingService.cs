using Grpc.Net.Client;
using VotingSystem.Voting;

namespace VotingApp.Services
{
    public class VotingService
    {
        private readonly VotingServiceClient _client;
        
        public VotingService()
        {
            var handler = Config.GetHttpClientHandler();
            var channel = GrpcChannel.ForAddress(Config.GetEndpoint(),
                new GrpcChannelOptions { HttpHandler = handler });
            _client = new VotingServiceClient(channel);
        }
        
        public async Task<List<Candidate>> GetCandidatesAsync()
        {
            try
            {
                var response = await _client.GetCandidatesAsync(new GetCandidatesRequest());
                return response.Candidates.Select(c => new Candidate
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList();
            }
            catch (Exception)
            {
                return GetDefaultCandidates();
            }
        }
        
        public async Task<VoteResult> CastVoteAsync(string credential, int candidateId)
        {
            try
            {
                var response = await _client.VoteAsync(new VoteRequest
                {
                    VotingCredential = credential,
                    CandidateId = candidateId
                });
                
                return new VoteResult
                {
                    Success = response.Success,
                    Message = response.Message
                };
            }
            catch (Exception ex)
            {
                return new VoteResult
                {
                    Success = false,
                    Message = $"Erro ao votar: {ex.Message}"
                };
            }
        }
        
        private List<Candidate> GetDefaultCandidates()
        {
            return Config.Settings.Candidates.DefaultCandidates
                .Select(c => new Models.Candidate
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList();
        }
    }
    
    public class VoteResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}