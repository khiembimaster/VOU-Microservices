
namespace QuizGame.Application.QuizGame.Commands.ScheduleWorkflow
{
    public class ScheduleWorkflowHandler
        //(IApplicationDbContext dbContext)
        : ICommandHandler<ScheduleWorkflowCommand, ScheduleWorkflowResult>
    {
        public async Task<ScheduleWorkflowResult> Handle(ScheduleWorkflowCommand command, CancellationToken cancellationToken)
        {
            // Get quizz from db
            // Get questions
            // Foreach question 
            //    Chain question
        }
    }
}
