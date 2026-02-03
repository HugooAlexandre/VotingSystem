using VotingSystem.Voting;

namespace VotingApp.Models
{
    public class VoteResult
    {
        public int CandidateId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public int Votes { get; set; }
        public double Percentage { get; set; }
        public DateTime Timestamp { get; set; }
        
        public VoteResult()
        {
            Timestamp = DateTime.Now;
        }
        
        public static List<VoteResult> FromGrpcResults(List<CandidateResult> grpcResults)
        {
            var results = new List<VoteResult>();
            int totalVotes = grpcResults.Sum(r => r.Votes);
            
            foreach (var grpcResult in grpcResults)
            {
                double percentage = totalVotes > 0 ? (grpcResult.Votes * 100.0) / totalVotes : 0;
                results.Add(new VoteResult
                {
                    CandidateId = grpcResult.Id,
                    CandidateName = grpcResult.Name,
                    Votes = grpcResult.Votes,
                    Percentage = percentage
                });
            }
            
            return results;
        }
        
        public override string ToString()
        {
            return $"{CandidateName}: {Votes} votos ({Percentage:F2}%)";
        }
    }
}