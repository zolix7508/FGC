using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Core.ErrorHandling
{
    public class ResultSet<TCode, TResult, TContext>: IEnumerable<TResult>
        where TCode : struct
        where TResult : ResultBase<TCode, TContext>, IResult<TCode, TContext>, new()
    {
        #region Properties
        private List<TResult> _results = new List<TResult>();

        public IResult<TCode, TContext> this[int index]
        {
            get
            {
                return _results[index];
            }
        }

        #endregion

        #region Methods
        public ResultSet<TCode, TResult, TContext> Add(TCode code)
        {
            var result = new TResult { ResultCode = code };
            _results.Add(result);
            return this;
        }

        public void Throw(TCode resultCode)
        {
            TResult result = new TResult();
            result.ResultCode = resultCode;
            this._results.Add(result);
            throw new AppExceptionBase<TCode, TResult, TContext>((TCode)Enum.Parse(typeof(TCode), resultCode.ToString()));
        }
        #endregion


        public IEnumerator<TResult> GetEnumerator()
        {
            return this._results.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._results.GetEnumerator();
        }
    }
}
