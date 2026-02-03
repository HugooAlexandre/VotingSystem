namespace VotingApp.Models
{
    public class Candidate
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Party { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        public Candidate() { }
        
        public Candidate(int id, string name, string party = "", string description = "")
        {
            Id = id;
            Name = name;
            Party = party;
            Description = description;
        }
        
        public override string ToString()
        {
            return string.IsNullOrEmpty(Party) ? 
                $"{Id}. {Name}" : 
                $"{Id}. {Name} ({Party})";
        }
        
        public bool IsValid()
        {
            return Id > 0 && !string.IsNullOrWhiteSpace(Name);
        }
    }
}