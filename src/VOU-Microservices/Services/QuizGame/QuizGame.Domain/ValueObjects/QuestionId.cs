﻿using QuizGame.Domain.Exceptions;

namespace QuizGame.Domain.ValueObjects
{
    public record QuestionId
    {
        public Guid Value { get; }
        private QuestionId(Guid value) => Value = value;
        public static QuestionId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value == Guid.Empty)
            {
                throw new DomainException("QuestionId cannot be empty.");
            }

            return new QuestionId(value);
        }
    }
}