namespace QuizGame.Domain.Models
{
    public class Answer : Entity<AnswerId>
    {
        public ParticipantId ParticipantId { get; private set; } = default;
        public ChoiceId ChoiceId { get; private set; } = default;
        
        public Answer Create(AnswerId answerId, ParticipantId participantId, ChoiceId choiceId)
        {
            var answer = new Answer
            {
                Id = answerId,
                ParticipantId = participantId,
                ChoiceId = choiceId
            };

            return answer;
        }
    }
}
