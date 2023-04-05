using DogGo.Models;
using Microsoft.Data.SqlClient;
using DogGo.Utils;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class WalkerRepository : BaseRepository, IWalkerRepository
    {
        public WalkerRepository(IConfiguration configuration) : base(configuration) { }

        public List<Walker> GetAllWalkers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT  w.Id as WalkerId, 
                                w.[Name] as WalkerName, 
                                w.ImageUrl as WalkerAvatar, 
                                n.Id as NeighborhoodId, 
                                n.Name as NeighborhoodName
                        FROM Walker w
                        JOIN Neighborhood n on w.NeighborhoodId = n.Id";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walker> walkers = new List<Walker>();
                    while (reader.Read())
                    {
                        Walker walker = new Walker
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            Name = reader.GetString(reader.GetOrdinal("WalkerName")),
                            ImageUrl = reader.GetString(reader.GetOrdinal("WalkerAvatar")),
                            Neighborhood = new Neighborhood
                            {
                                Id = DbUtils.GetInt(reader, "NeighborhoodId"),
                                Name = DbUtils.GetString(reader, "NeighborhoodName")
                            }
                        };

                        walkers.Add(walker);
                    }

                    reader.Close();

                    return walkers;
                }
            }
        }

        public Walker GetWalkerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT  w.Id as WalkerId,
                            w.[Name] as WalkerName,
                            w.ImageUrl as WalkerAvatar,
                            n.Id as NeighborhoodId,
                            n.Name as NeighborhoodName
                    FROM Walker w
                            JOIN Neighborhood n on w.NeighborhoodId = n.Id
                    WHERE w.Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        Walker walker = new Walker
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            Name = reader.GetString(reader.GetOrdinal("WalkerName")),
                            ImageUrl = reader.GetString(reader.GetOrdinal("WalkerAvatar")),
                            Neighborhood = new Neighborhood
                            {
                                Id = DbUtils.GetInt(reader, "NeighborhoodId"),
                                Name = DbUtils.GetString(reader, "NeighborhoodName")
                            }
                        };

                        reader.Close();
                        return walker;
                    }
                    else
                    {
                        reader.Close();
                        return null;
                    }
                }
            }
        }
    }
}