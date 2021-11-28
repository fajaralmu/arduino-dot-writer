namespace DotWriterServer.Dto
{
    public class WebResponse<T>
    {

        public int Success {get;set;}
        public System.DateTime Date {get;set;} = System.DateTime.Now;
        public string Message {get;set;}
        public T Result {get;set;}

        public WebResponse(T returnValue)
        {
            Result = returnValue;
        }

        public static WebResponse<T> SuccessResponse(T returnValue)
        {
            WebResponse<T> response = new WebResponse<T>(returnValue);
            return response;
        }
    }
}