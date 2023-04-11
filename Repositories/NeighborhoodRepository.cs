using DogGo.Models;
using Microsoft.Data.SqlClient;
using DogGo.Utils;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class NeighborhoodRepository : BaseRepository, INeighborhoodRepository
    {
        public NeighborhoodRepository(IConfiguration configuration) : base(configuration) { }

        public List<Neighborhood> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name FROM Neighborhood";

                    var reader = cmd.ExecuteReader();

                    List<Neighborhood> neighborhoods = new List<Neighborhood>();

                    while (reader.Read())
                    {
                        Neighborhood neighborhood = new Neighborhood()
                        {
                            Id = DbUtils.GetInt(reader, "Id"),
                            Name = DbUtils.GetString(reader, "Name")
                        };
                        neighborhoods.Add(neighborhood);
                    }

                    reader.Close();

                    return neighborhoods;
                }
            }
        }
    }
}