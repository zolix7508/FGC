using Infrastructure.Core.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Core.ErrorHandling
{
    public partial class ResultBase<TCode, TContext>
        where TCode : struct
    {
        #region Fields
        protected TCode _resultCode;
        protected ResultType _resultType;
        protected string _message;
        protected TContext _data;

        #endregion

        #region Properties
        public string Message
        {
            get { return _message; }
        }

        public TCode ResultCode
        {
            get { return _resultCode; }
            internal set
            {
                _resultCode = value;
                Init();
            }
        }

        public ResultType ResultType
        {
            get { return _resultType; }
        }

        public bool IsError
        {
            get
            {
                return _resultType == ResultType.Error;
            }
        }

        public TContext Context
        {
            get
            {
                return _data;
            }
            internal set
            {
                _data = value;
            }
        }
        #endregion

        #region Constructors

        protected ResultBase()
        {
            _resultType = ResultType.Error;
            _message = String.Empty;
            _data = default(TContext);
        }

        #endregion

        protected void Init()
        {
            var result = this as IResult<TCode, TContext>;
            _resultType = result.GetResultType(_resultCode);
            _message = result.GetMessage(_resultCode, _data);
        }


    }
}
