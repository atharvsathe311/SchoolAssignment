using SchoolApi.Core.GenearalModels;

namespace SchoolAPI.Constants
{
    public class ErrorMessages
    {
        public const string ValidationError = "Invalid Inputs";
        public const string STUDENT_NOT_FOUND = "Student Not Found.";
        public const string STUDENT_CREATE_FAILED = "Failed to create student.";
        public const string NOTHING_TO_UPDATE = "Nothing to Update.";
        public const string STUDENT_EXISTS = "Student Already Exist";         
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
        public const string BIRTH_DATE_REQUIRED = "BirthDate is Required";
        public const string INVALID_BIRTH_DATE = "Invalid Birthdate";
        public static ErrorDetails StudentNotFoundExceptionDetails =  new ErrorDetails() 
        {
            Message = STUDENT_NOT_FOUND ,
            StatusCode = StatusCodes.Status404NotFound
        };

        public static ErrorDetails StudentExistsExceptionDetails =  new ErrorDetails() 
        {
            Message = STUDENT_EXISTS ,
            StatusCode = StatusCodes.Status400BadRequest
        };
        public static ErrorDetails NothingToUpdate =  new ErrorDetails() 
        {
            Message = NOTHING_TO_UPDATE ,
            StatusCode = StatusCodes.Status400BadRequest
        };
    }
}