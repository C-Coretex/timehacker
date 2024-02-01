using System;

namespace Helpers.Domain.Models
{
    public class Response
    {
        public bool Success { get; set; } = true;
        public string Error { get; set; }
        public string StackTrace { get; set; }
        public object Data { get; set; } = null;


        public Response() {}

        public Response(Exception ex)
        {
            InitializeException(ex);
        }

        public Response(Func<object> func, string error = null)
        {
            try
            {
                func = func ?? throw new ArgumentNullException(nameof(func));
                Data = func();
            }
            catch (Exception ex)
            {
                InitializeException(ex, error);
            }
        }

        public Response(Action func, string error = null)
        {
            try
            {
                func = func ?? throw new ArgumentNullException(nameof(func));
                func();
            }
            catch (Exception ex)
            {
                InitializeException(ex, error);
            }
        } 

        private void InitializeException(Exception ex, string error = null)
        {
            Success = false;
            Error = error ?? ex.InnerException?.Message ?? ex.Message;
            StackTrace = ex.InnerException?.StackTrace ?? ex.StackTrace;
        }
    }
}
