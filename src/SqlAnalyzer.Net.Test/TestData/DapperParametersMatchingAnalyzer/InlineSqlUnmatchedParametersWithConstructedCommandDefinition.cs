using System.Data.SqlClient;
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
            var sqlText = "\"inline sql @param, @not_found\"";
            var param = new { param = "some_string" };
            var cmd = new CommandDefinition(sqlText, param);

            sql.Execute(cmd);
        }
    }
}
