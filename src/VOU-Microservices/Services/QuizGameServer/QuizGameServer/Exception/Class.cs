using BuildingBlocks.Exceptions;

namespace QuizGame.Api.Exception
{
    public class JoinGameException : BadRequestException
    {
        public JoinGameException(string message, string details) : base(message, details)
        {
        }
    }
}
