﻿using BookStoreDataAccess.Data;
using BookStoreDataAccess.Repository.IRepository;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookStoreDataAccess.Repository
{
    class StoredProcedureCall : IStoredProcedureCall
    {
        private readonly ApplicationDbContext _db;
        private static string ConnectionString="";
        public StoredProcedureCall(ApplicationDbContext db)
        {
            _db = db;
            ConnectionString = db.Database.GetDbConnection().ConnectionString; 
            /*because here, we don't use DbSet to carry queries to and fro the db. Stored Procedures
             * are named group of queries so, we give only a name(string) instead of a query and so
             * we don't use DbSet here*/
        }
        public void Dispose()
        {
            _db.Dispose();
        }

        public void Execute(string procedureName, DynamicParameters param = null)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            sqlConnection.Execute(procedureName, param, commandType: System.Data.CommandType.StoredProcedure);
        }

        public IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            return sqlConnection.Query<T>(procedureName, param, 
                commandType: System.Data.CommandType.StoredProcedure);
        }

        //stored procedure to retreive 2 tables
        public Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters param = null)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            var result = SqlMapper.QueryMultiple(sqlConnection, procedureName, param, 
                commandType: System.Data.CommandType.StoredProcedure);
            var item1 = result.Read<T1>().ToList();
            var item2 = result.Read<T2>().ToList();
            if(item1!=null && item2!=null)
            {
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(item1, item2);
            }
            return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(new List<T1>(), new List<T2>());
        }

        public T OneRecord<T>(string procedureName, DynamicParameters param = null)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            var value = sqlConnection.Query<T>(procedureName, param, 
                commandType: System.Data.CommandType.StoredProcedure);
            return (T)Convert.ChangeType(value.FirstOrDefault(), typeof(T));
        }

        public T Single<T>(string procedureName, DynamicParameters param = null)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            return (T)Convert.ChangeType(sqlConnection.Execute(procedureName, param,
                commandType: System.Data.CommandType.StoredProcedure), typeof(T));
        }
    }
}
