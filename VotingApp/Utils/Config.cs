using System.Text.Json;

namespace VotingApp.Utils
{
    public static class Config
    {
        private const string ConfigFile = "appsettings.json";
        private static AppSettings? _settings;
        
        public class AppSettings
        {
            public VotingSystemConfig VotingSystem { get; set; } = new();
            public CredentialsConfig Credentials { get; set; } = new();
            public CandidatesConfig Candidates { get; set; } = new();
        }
        
        public class VotingSystemConfig
        {
            public string Endpoint { get; set; } = "https://ken01.utad.pt:9091";
            public int TimeoutSeconds { get; set; } = 30;
            public int MaxRetries { get; set; } = 3;
            public bool MockMode { get; set; } = false;
        }
        
        public class CredentialsConfig
        {
            public string[] ValidCredentials { get; set; } = 
                new[] { "CRED-ABC-123", "CRED-DEF-456", "CRED-GHI-789" };
        }
        
        public class CandidatesConfig
        {
            public CandidateInfo[] DefaultCandidates { get; set; } = Array.Empty<CandidateInfo>();
        }
        
        public class CandidateInfo
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
        
        public static AppSettings Settings
        {
            get
            {
                if (_settings == null) LoadSettings();
                return _settings!;
            }
        }
        
        private static void LoadSettings()
        {
            try
            {
                if (File.Exists(ConfigFile))
                {
                    string json = File.ReadAllText(ConfigFile);
                    _settings = JsonSerializer.Deserialize<AppSettings>(json);
                }
                else
                {
                    _settings = new AppSettings();
                }
            }
            catch
            {
                _settings = new AppSettings();
            }
        }
        
        public static string GetEndpoint() => Settings.VotingSystem.Endpoint;
        
        public static bool IsValidCredential(string credential)
        {
            return Settings.Credentials.ValidCredentials.Contains(credential);
        }
        
        public static HttpClientHandler GetHttpClientHandler()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = 
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            return handler;
        }
    }
}