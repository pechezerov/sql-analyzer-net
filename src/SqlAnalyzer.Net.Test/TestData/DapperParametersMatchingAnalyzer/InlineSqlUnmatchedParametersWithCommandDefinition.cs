﻿using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

using Dapper;

namespace Sql.Analyzer.Test.TestData
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var sql = new SqlConnection();
            var cmd = new CommandDefinition("inline sql @param, @not_found",
                new { param = "some_string" });

            sql.Execute(cmd);
        }
    }
}
