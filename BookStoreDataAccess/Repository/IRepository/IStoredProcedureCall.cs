using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreDataAccess.Repository.IRepository
{
    public interface IStoredProcedureCall : IDisposable
    {
        T Single<T>(string procedureName, DynamicParameters param = null);/*Return a single value
                * Dapper to balance all the parameters */

        void Execute(string procedureName, DynamicParameters param = null);/* For common functions like
                * Add, delete, etc. Doesn't return any value */
        T OneRecord<T>(string procedureName, DynamicParameters param = null);/* Retreive one complete record
                 * D/B Single and OneRecord, we'll use ExecuteScalar that returns integer or boolean value in Single
                 * and in OneRecord we retreive the complete row. */
        IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null);//to retrieve all records
        Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters param = null);
    }
}
