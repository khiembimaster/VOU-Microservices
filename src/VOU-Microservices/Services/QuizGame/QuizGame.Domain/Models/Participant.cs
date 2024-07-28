namespace QuizGame.Domain.Models
{
    public class Participant : Entity<ParticipantId>
    {
        public string Name { get; private set; } = default;
        public string Email { get; private set; } = default;
        
        public static Participant Create(ParticipantId participantId, string name, string email)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(email);

            var participant = new Participant
            {
                Id = participantId,
                Name = name,
                Email = email
            };

            return participant;
        }
    }
}