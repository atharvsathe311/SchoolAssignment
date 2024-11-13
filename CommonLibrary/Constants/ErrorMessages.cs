using CommonLibrary.GeneralModels;

namespace CommonLibrary.Constants
{
    public class ErrorMessages
    {
        public const string ValidationError = "Invalid Inputs";
        public const string USER_NOT_FOUND = "User Not Found.";
        public const string USER_CREATE_FAILED = "Failed to create user.";
        public const string NOTHING_TO_UPDATE = "Nothing to Update.";
        public const string USER_EXISTS = "User Already Exist";         
        public const string INTERNAL_SERVER_ERROR = "Internal Server Error !";
        public const string FIRST_NAME_REQUIRED = "FirstName is Required";
        public const string FIRST_NAME_MIN_LENGTH = "FirstName must be at least 2 Characters";
        public const string FIRST_NAME_MAX_LENGTH = "FirstName cannot exceed 15 Characters";
        public const string LAST_NAME_REQUIRED = "LastName is Required";
        public const string LAST_NAME_MIN_LENGTH = "LastName must be at least 2 Characters";
        public const string LAST_NAME_MAX_LENGTH = "LastName cannot exceed 15 Characters";
        public const string EMAIL_REQUIRED = "Email is Required";
        public const string INVALID_EMAIL = "Invalid Email";
        public const string PHONE_REQUIRED = "Phone Number is Required";
        public const string INVALID_PHONE_NUMBER = "Invalid Phone Number";
        public const string EMPTY_USERNAME = "Username is required";
        public const string EMPTY_PASSWORD = "Password is required";
        public const string USERNAME_VALIDATION = "Username must contain at least one letter, one number, and be at least 6 characters long.";
        public const string PASSWORD_VALIDATION = "Password must be at least 8 characters long and contain at least one letter, one number, and one special character.";
        public const string INVALID_CREDENTIALS = "Invalid Credentials.";
        public static ErrorDetails UserNotFoundExceptionDetails =  new ErrorDetails() 
        {
            Message = USER_NOT_FOUND ,
            StatusCode = StatusCodes.Status404NotFound
        };
        public static ErrorDetails UserExistsExceptionDetails =  new ErrorDetails() 
        {
            Message = USER_NOT_FOUND ,
            StatusCode = StatusCodes.Status400BadRequest
        };
        public static ErrorDetails NothingToUpdate =  new ErrorDetails() 
        {
            Message = NOTHING_TO_UPDATE ,
            StatusCode = StatusCodes.Status400BadRequest
        };
        public static ErrorDetails InvalidCredentialsExceptionDetails =  new ErrorDetails() 
        {
            Message = INVALID_CREDENTIALS,
            StatusCode = StatusCodes.Status400BadRequest
        };

        

    }
}