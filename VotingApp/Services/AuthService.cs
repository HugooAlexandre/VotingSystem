using Grpc.Net.Client;
using VotingSystem;

namespace VotingApp.Services
{
    public class AuthService
    {
        private readonly VoterRegistrationService.VoterRegistrationServiceClient _client;
        
        public AuthService()
        {
            var handler = Config.GetHttpClientHandler();
            var channel = GrpcChannel.ForAddress(Config.GetEndpoint(), 
                new GrpcChannelOptions { HttpHandler = handler });
            _client = new VoterRegistrationService.VoterRegistrationServiceClient(channel);
        }
        
        public async Task<AuthenticationResult> AuthenticateVoter(string citizenCardNumber)
        {
            try
            {
                var response = await _client.IssueVotingCredentialAsync(
                    new VoterRequest { CitizenCardNumber = citizenCardNumber });
                
                return new AuthenticationResult
                {
                    IsSuccess = response.IsEligible,
                    Credential = response.VotingCredential,
                    Message = response.IsEligible ? 
                        "Credencial emitida com sucesso" : 
                        "Eleitor inelegível"
                };
            }
            catch (Exception ex)
            {
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    Credential = string.Empty,
                    Message = $"Erro de autenticação: {ex.Message}"
                };
            }
        }
        
        public bool ValidateCredential(string credential)
        {
            return Config.IsValidCredential(credential);
        }
    }
    
    public class AuthenticationResult
    {
        public bool IsSuccess { get; set; }
        public string Credential { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}