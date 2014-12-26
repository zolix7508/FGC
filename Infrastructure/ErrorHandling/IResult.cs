using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Core.ErrorHandling
{
    public enum ResultType : int
    {
        Error = 0,
        Warning = 1,
        Success = 2
    }

    public interface IResult<TCode, TContext> where TCode : struct//, IConvertible  
    {
        string GetMessage(TCode resultCode, TContext context = default(TContext));
        ResultType GetResultType(TCode resultCode);

        TCode ResultCode { get; }
        bool IsError { get; }
        string Message { get; }
        ResultType ResultType { get; }
        TContext Context { get; }
    }
}
