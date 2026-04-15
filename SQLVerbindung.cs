using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace AppVorlage
{
    public class SQLVerbindung
    {
        private readonly string connectionString;

        public SQLVerbindung()
        {
            connectionString =
                "Server=localhost\\SQLEXPRESS;Database=AdventureWorks2025;Trusted_Connection=True;TrustServerCertificate=True;";
        }

        public List<EmployeeInfo> SearchEmployees(
            string? employeeIdKeyword,
            string? name,
            string? jobTitle,
            DateTime? hireDateFrom)
        {
            var list = new List<EmployeeInfo>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var sql = @"
                    SELECT
                        e.BusinessEntityID,
                        p.FirstName,
                        p.LastName,
                        e.JobTitle,
                        e.HireDate
                    FROM HumanResources.Employee e
                    JOIN Person.Person p 
                        ON e.BusinessEntityID = p.BusinessEntityID
                    WHERE 1=1";

                if (!string.IsNullOrWhiteSpace(employeeIdKeyword))
                    sql += " AND CAST(e.BusinessEntityID AS NVARCHAR(20)) LIKE @employeeId";
                
                if (!string.IsNullOrWhiteSpace(name))
                    sql += " AND (p.FirstName LIKE @name OR p.LastName LIKE @name)";

                if (!string.IsNullOrWhiteSpace(jobTitle))
                    sql += " AND e.JobTitle = @jobTitle";

                if (hireDateFrom.HasValue)
                    sql += " AND e.HireDate >= @hireDateFrom";

                using var cmd = new SqlCommand(sql, connection);

                if (!string.IsNullOrWhiteSpace(employeeIdKeyword))
                    cmd.Parameters.AddWithValue("@employeeId", "%" + employeeIdKeyword + "%");

                if (!string.IsNullOrWhiteSpace(name))
                    cmd.Parameters.AddWithValue("@name", "%" + name + "%");

                if (!string.IsNullOrWhiteSpace(jobTitle))
                    cmd.Parameters.AddWithValue("@jobTitle", jobTitle);

                if (hireDateFrom.HasValue) // HasValue 是 Nullable<T> 结构的一个属性，表示是否有值 可空变量
                    cmd.Parameters.AddWithValue("@hireDateFrom", hireDateFrom.Value);

                using var reader = cmd.ExecuteReader(); // 把 SQL 发给数据库执行，然后返回一个结果读取器

                while (reader.Read())
                {
                    list.Add(new EmployeeInfo
                    {
                        BusinessEntityID = (int)reader["BusinessEntityID"],
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        JobTitle = reader["JobTitle"].ToString(),
                        HireDate = (DateTime)reader["HireDate"]
                    });
                }
            }

            return list;
        }

        public List<string> GetJobTitles()
        {
            var list = new List<string>();

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using var cmd = new SqlCommand(@"
                SELECT DISTINCT JobTitle
                FROM HumanResources.Employee", conn);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                list.Add(reader["JobTitle"].ToString());

            return list;
        }
    }
}