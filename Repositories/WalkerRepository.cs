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
                            NeighborhoodId = DbUtils.GetInt(reader, "NeighborhoodId"),
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

        public void AddWalker(Walker walker)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO Walker ([Name], NeighborhoodId, ImageUrl)
                    OUTPUT INSERTED.ID
                    VALUES (@name, @neighborhoodId, @imageUrl);";

                    cmd.Parameters.AddWithValue("@name", walker.Name);
                    cmd.Parameters.AddWithValue("@neighborhoodId", walker.NeighborhoodId);
                    cmd.Parameters.AddWithValue("@imageUrl", walker.ImageUrl);

                    int id = (int)cmd.ExecuteScalar();

                    walker.Id = id;
                }
            }
        }

        public void UpdateWalker(Walker walker)
        {
            using (var conn = Connection)
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    UPDATE Walker
                    SET
                        [Name] = @name,
                        NeighborhoodId = @neighborhoodId,
                        ImageUrl = @imageUrl
                    WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@name", walker.Name);
                    cmd.Parameters.AddWithValue("@neighborhoodId", walker.NeighborhoodId);
                    cmd.Parameters.AddWithValue("@imageUrl", walker.ImageUrl);
                    cmd.Parameters.AddWithValue("@id", walker.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteWalker(int walkerId)
        {
            using (var conn = Connection)
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    DELETE FROM Walker 
                    WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@id", walkerId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Walker> GetWalkersInNeighborhood(int neighborhoodId)
        {
            using (var conn = Connection)
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT  Id, 
                            [Name], 
                            ImageUrl, 
                            NeighborhoodId
                    FROM Walker
                    WHERE NeighborhoodId = @neighborhoodId";

                    cmd.Parameters.AddWithValue("@neighborhoodId", neighborhoodId);

                    var reader = cmd.ExecuteReader();

                    List<Walker> walkers = new List<Walker>();

                    while (reader.Read())
                    {
                        Walker walker = new Walker
                        {
                            Id = DbUtils.GetInt(reader, "Id"),
                            Name = DbUtils.GetString(reader, "Name"),
                            ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                            NeighborhoodId = DbUtils.GetInt(reader, "NeighborhoodId")
                        };

                        walkers.Add(walker);
                    }

                    reader.Close();

                    return walkers;
                }
            }
        }
    }
}