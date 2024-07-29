

namespace QuizGame.Domain.Models
{
    public class Quiz : Aggregate<QuizId>
    {
        private readonly List<QuizQuestion> _quizQuestions = new();
        public IReadOnlyList<QuizQuestion> QuizQuestions => _quizQuestions.AsReadOnly();
        
        //private readonly List<Participant> _participants = new();
        //public IReadOnlyList<Participant> Participants => _participants.AsReadOnly();
        //public LeaderBoardId LeaderBoardId { get; private set; } = default;

        public QuizTitle Title { get; private set; } = default;
        public QuizDescription Description { get; private set; } = default;
        public DateTime StartTime { get; private set; } = default;
        public DateTime EndTime { get; private set; } = default;
        public QuizStatus Status { get; private set;} = default;
        public int TotalPoint
        {
            get => QuizQuestions.Sum(x => x.Score);
            private set { }
        }

        public static Quiz Create(QuizId quizId, QuizTitle quizTitle, QuizDescription quizDescription, DateTime startTime)
        {
            var quiz = new Quiz
            {
                Id = quizId,
                Title = quizTitle,
                Description = quizDescription,
                StartTime = startTime,
                Status = QuizStatus.NotStarted,
            };

            quiz.AddDomainEvent(new QuizCreatedEvent(quiz));

            return quiz;
        }

        public void Update(QuizTitle quizTitle, QuizDescription quizDescription, DateTime startTime, DateTime endTime, QuizStatus status)
        {
            Title = quizTitle;
            Description = quizDescription;
            StartTime = startTime;
            EndTime = endTime;
            Status = status;

            AddDomainEvent(new QuizUpdatedEvent(this));
        }

        public void AddQuestion(QuestionId questionId, int score)
        {
            var quizQuestion = new QuizQuestion(Id, questionId, score);

            _quizQuestions.Add(quizQuestion);
        }

        public void RemoveQuestion(QuestionId questionId)
        {
            var quizQuestion = _quizQuestions.FirstOrDefault(x => x.QuestionId == questionId);
            
            if (quizQuestion is not null)
            {
                _quizQuestions.Remove(quizQuestion);
            }
        }

    }
}
