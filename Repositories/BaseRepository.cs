using Microsoft.Data.SqlClient;

namespace DogGo.Repositories
{
    public class BaseRepository
    {
        private string _connectionString;

        public BaseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        protected SqlConnection Connection => new SqlConnection(_connectionString);
    }
}