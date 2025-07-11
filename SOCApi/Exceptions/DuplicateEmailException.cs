using System;

namespace SOCApi.Exceptions
{
    public class DuplicateEmailException : Exception
    {

        public DuplicateEmailException(string emailAddress)
            : base($"The email address '{emailAddress}' is already registered.")
        {
        }
    }
}
