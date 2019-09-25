using GroupPlus.Business.Infrastructure.Contract;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupPlus.Business.Infrastructure
{
    public class GenericRepository<T> : IReaderRepository<T> where T : class
    {
       
        public T bindReaderToObj(DbDataReader dr)
        {
           
            var items = new List<T>();
            foreach (object item in items)
            {
                //NYSCDetailId = dr.GetFieldValue<int>(dr.GetOrdinal("NYSCDetailId")),
                item = dr.GetFieldValue<object>(dr.GetOrdinal(item.GetType().Name.ToString()));
            }
          
        }

       
    }
}
