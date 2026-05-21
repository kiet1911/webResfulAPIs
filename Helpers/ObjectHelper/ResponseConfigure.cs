namespace webResfulAPIs.Helpers.ObjectHelper
{
    sealed public class ResponseConfigure
    {
        public ResponseConfigure()
        {

        }

        public object CustomResponse( string? error , string message , string statusCode ){

            return new { 
                Error = error??"",
                Message = message,
                StatusCode = statusCode
            };
        }
    }
}
