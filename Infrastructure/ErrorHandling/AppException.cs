using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Core.ErrorHandling
{
    public class AppExceptionBase<TCode, TResult, TContext> : Exception
        where TResult : IResult<TCode, TContext>, new()
        where TCode : struct
    {
        private TResult _result;

        public TResult Result
        {
            get
            {
                return _result;
            }
        }

        protected AppExceptionBase() { }

        protected AppExceptionBase(TCode code, TContext context)
            : base(new TResult().GetMessage(code, context))
        {
            _result = new TResult();
            var resultBase = _result as ResultBase<TCode, TContext>;
            resultBase.Context = context;
            resultBase.ResultCode = code;
        }

        internal AppExceptionBase(TCode code)
            : this(code, default(TContext))
        { }
    }
}
