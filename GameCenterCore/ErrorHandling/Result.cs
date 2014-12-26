using Infrastructure.Core.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenterCore.ErrorHandling
{
    public class Results : ResultSet<ResultCode, Result, object> { }

    public class Result : ResultBase<ResultCode, object>, IResult<ResultCode, object>//IResult
    {
        public string GetMessage(ResultCode resultCode, object context)
        {
            return resultCode.ToString();
        }

        public ResultType GetResultType(ResultCode resultCode)
        {
            return ResultType.Error;
        }
    }
}
