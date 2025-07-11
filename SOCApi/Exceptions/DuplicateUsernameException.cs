using System;

namespace SOCApi.Exceptions
{
    public class DuplicateUsernameException : Exception
    {
        public DuplicateUsernameException() { }

        public DuplicateUsernameException(string? message) : base(message) { }

        public DuplicateUsernameException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}

