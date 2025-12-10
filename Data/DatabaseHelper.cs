using Microsoft.Data.SqlClient;
using System.Data;
namespace projectPart1.Data
{
    public class DatabaseHelper
    {
        private readonly string connectionString;
        private readonly ILogger<DatabaseHelper> logger;
        public DatabaseHelper(IConfiguration configuration, ILogger<DatabaseHelper> logger)
        {
            this.connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");
            this.logger = logger;
        }
        public async Task<DataTable> ExecuteQueryAsync(string query, SqlParameter[]? parameters = null)
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.Text;
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
                logger.LogInformation("Query executed successfully. Rows returned: {RowCount}", dataTable.Rows.Count);
                return dataTable;
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "SQL Error executing query: {Query}", query);
                throw new InvalidOperationException("Database query failed. Please check your connection.", ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing query: {Query}", query);
                throw;
            }
        }
        public async Task<int> ExecuteNonQueryAsync(string query, SqlParameter[]? parameters = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.Text;
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        logger.LogInformation("Non-query executed successfully. Rows affected: {RowsAffected}", rowsAffected);
                        return rowsAffected;
                    }
                }
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "SQL Error executing non-query: {Query}", query);
                throw new InvalidOperationException("Database operation failed. Please try again.", ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing non-query: {Query}", query);
                throw;
            }
        }
        public async Task<object?> ExecuteScalarAsync(string query, SqlParameter[]? parameters = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.Text;
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        object? result = await command.ExecuteScalarAsync();
                        logger.LogInformation("Scalar query executed successfully");
                        return result;
                    }
                }
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "SQL Error executing scalar query: {Query}", query);
                throw new InvalidOperationException("Database query failed.", ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing scalar query: {Query}", query);
                throw;
            }
        }
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    logger.LogInformation("Database connection successful");
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database connection failed");
                return false;
            }
        }
        public async Task<SqlDataReader> ExecuteReaderAsync(string query, SqlParameter[]? parameters = null)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(query, connection);
                command.CommandType = CommandType.Text;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                logger.LogInformation("DataReader query executed successfully");
                return reader;
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "SQL Error executing reader query: {Query}", query);
                throw new InvalidOperationException("Database query failed.", ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing reader query: {Query}", query);
                throw;
            }
        }
    }
}