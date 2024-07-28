namespace QuizGame.Domain.Models
{
    public class Game : Aggregate<GameId>
    {
        private readonly List<GameQuestion> _gameQuestions = new();
        public IReadOnlyList<GameQuestion> GameQuestions => _gameQuestions.AsReadOnly();
        
        //private readonly List<Participant> _participants = new();
        //public IReadOnlyList<Participant> Participants => _participants.AsReadOnly();

        public LeaderBoardId LeaderBoardId { get; private set; } = default;

        public GameTitle GameTitle { get; private set; } = default;
        public GameDescription GameDescription { get; private set; } = default;
        public DateTime StartTime { get; private set; } = default;
        public DateTime EndTime { get; private set; } = default;
        public GameStatus GameStatus { get; private set;} = default;
    }
}
