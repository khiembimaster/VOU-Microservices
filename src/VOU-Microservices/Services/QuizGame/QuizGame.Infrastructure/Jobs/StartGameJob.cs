using MediatR;

namespace QuizGame.Application.Jobs
{
    public class StartGameJob
        (ISender sender)
        : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var gameId = context.MergedJobDataMap.GetGuid("game-id");
           
        }
    }
}
