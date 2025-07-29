using System.Data;

namespace BlazorApp.Services
{
    public interface IDapperService
    {
        T QuerySingle<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text);
        Task<T> QuerySingleAsync<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text);

        T? QuerySingleOrDefault<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text);
        Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text);

        IEnumerable<T> Query<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text);

        int Execute(string sql, object? parameters = null, CommandType commandType = CommandType.Text);
        Task<int> ExecuteAsync(string sql, object? parameters = null, CommandType commandType = CommandType.Text);
    }
}
