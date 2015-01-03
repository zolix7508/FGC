using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Infrastructure.Core.Serialization
{
    public class Serialize
    {
        public static string ToJson(object value)
        {
            return new JavaScriptSerializer().Serialize(value);
        }

        public static T FromJson<T>(string serialized)
        {
            return new JavaScriptSerializer().Deserialize<T>(serialized);
        }
    }
}
