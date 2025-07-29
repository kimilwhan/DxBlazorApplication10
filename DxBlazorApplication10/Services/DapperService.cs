using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BlazorApp.Services
{
    public class DapperService : IDapperService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public T QuerySingle<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text)
        {
            using var connection = CreateConnection();
            return connection.QuerySingle<T>(sql, parameters, commandType: commandType);
        }

        public async Task<T> QuerySingleAsync<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text)
        {
            using var connection = CreateConnection();
            return await connection.QuerySingleAsync<T>(sql, parameters, commandType: commandType);
        }

        // --- 추가된 메서드 (결과 없으면 null 반환) ---
        public T? QuerySingleOrDefault<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text)
        {
            using var connection = CreateConnection();
            return connection.QuerySingleOrDefault<T>(sql, parameters, commandType: commandType);
        }

        // --- ✨ 오류 해결을 위해 추가된 핵심 메서드 ---
        public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text)
        {
            using var connection = CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<T>(sql, parameters, commandType: commandType);
        }

        public IEnumerable<T> Query<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text)
        {
            using var connection = CreateConnection();
            return connection.Query<T>(sql, parameters, commandType: commandType);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<T>(sql, parameters, commandType: commandType);
        }

        public int Execute(string sql, object? parameters = null, CommandType commandType = CommandType.Text)
        {
            using var connection = CreateConnection();
            return connection.Execute(sql, parameters, commandType: commandType);
        }

        public async Task<int> ExecuteAsync(string sql, object? parameters = null, CommandType commandType = CommandType.Text)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteAsync(sql, parameters, commandType: commandType);
        }
    }
}
