using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Core.ErrorHandling
{
    public static class ExceptionExtensions
    {
        public static string MessageAll(this Exception ex)
        {
            string message = ex.Message;
            Exception ex2 = ex.InnerException;
            while (ex2 != null)
            {
                message = string.Join(Environment.NewLine, message, ex2.Message);
                ex2 = ex2.InnerException;
            }
            return message;
        }
    }
}
