namespace CafeReservationAPI.Constant
{
    public class ResponseMessages
    {
        public ResponseMessages(int code, string? controllerArea)
        {
            if (controllerArea == ControlerArea.AUTH)
            {
                Message = code switch
                {
                    400 => EMAIL_EXISTS,
                    401 => INVALID_LOGIN_DATA,
                    _ => ""
                };
            }
            else if (controllerArea == ControlerArea.ADMIN)
            {
                Message = code switch
                {
                    404 => USER_NOT_FOUND,
                    400 => VALIDITY_INVALID,
                    200 => UPDATED,
                    _ => ""
                };
            }
        }

        //Auth Controller
        public const string EMAIL_EXISTS = "Email already exists";
        public const string INVALID_LOGIN_DATA = "Invalid login data";

        //Admin Controller
        public const string USER_NOT_FOUND = "User not found";
        public const string VALIDITY_INVALID = "The validity is invalid.";
        public const string UPDATED = "Updated";


        public string? Message { get; set; }
    }
}
