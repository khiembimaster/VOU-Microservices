namespace QuizGame.Application.QuizGame.Commands.ScheduleWorkflow
{
    public record ScheduleWorkflowCommand(Guid QuizId) : ICommand<ScheduleWorkflowResult>;
    public record ScheduleWorkflowResult(bool IsSuccess);
}
