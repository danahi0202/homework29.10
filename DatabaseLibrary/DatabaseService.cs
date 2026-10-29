using System;
using System.Data;
using Dapper;
using System.Data.SqlClient;

namespace DatabaseLibrary
{
    public class DatabaseService
    {
        private readonly string connectionString =
            @"Server=DESKTOP-S6BK4K0;
              Database=ThreadCrudDb;
              Trusted_Connection=True;
              TrustServerCertificate=True;";

        public void InitializeDatabase()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Execute(@"
            IF OBJECT_ID('Products', 'U') IS NULL
            BEGIN
                CREATE TABLE Products
                (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(100) NOT NULL,
                    Price DECIMAL(10,2) NOT NULL
                );
            END");
            }
        }

        public int Create(Product product)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Products (Name, Price)
                       VALUES (@Name, @Price);
                       SELECT CAST(SCOPE_IDENTITY() AS INT);";

                return db.QuerySingle<int>(sql, product);
            }
        }

        public Product Read(int id)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.QuerySingleOrDefault<Product>(
                    "SELECT * FROM Products WHERE Id = @Id",
                    new { Id = id });
            }
        }

        public bool Update(Product product)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                int rows = db.Execute(
                    @"UPDATE Products
              SET Name = @Name, Price = @Price
              WHERE Id = @Id",
                    product);

                return rows > 0;
            }
        }

        public bool Delete(int id)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                int rows = db.Execute(
                    "DELETE FROM Products WHERE Id = @Id",
                    new { Id = id });

                return rows > 0;
            }
        }
    }
}