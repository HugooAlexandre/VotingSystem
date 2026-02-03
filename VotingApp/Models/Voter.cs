namespace VotingApp.Models
{
    public class Voter
    {
        public string CitizenCardNumber { get; set; } = string.Empty;
        public string VotingCredential { get; set; } = string.Empty;
        public bool HasVoted { get; set; } = false;
        public DateTime? VoteTimestamp { get; set; }
        public int? SelectedCandidateId { get; set; }
        
        public bool HasCredential => !string.IsNullOrEmpty(VotingCredential);
        public bool CanVote => HasCredential && !HasVoted;
        
        public void RecordVote(int candidateId)
        {
            HasVoted = true;
            SelectedCandidateId = candidateId;
            VoteTimestamp = DateTime.Now;
            VotingCredential = string.Empty; // Consumir credencial
        }
        
        public void Reset()
        {
            CitizenCardNumber = string.Empty;
            VotingCredential = string.Empty;
            HasVoted = false;
            VoteTimestamp = null;
            SelectedCandidateId = null;
        }
        
        public override string ToString()
        {
            return $"Eleitor CC: {CitizenCardNumber}, " +
                   $"Credencial: {(HasCredential ? "Obtida" : "Não obtida")}, " +
                   $"Votou: {(HasVoted ? "Sim" : "Não")}";
        }
    }
}