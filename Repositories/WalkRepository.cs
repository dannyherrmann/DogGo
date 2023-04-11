using DogGo.Models;
using DogGo.Utils;
using Microsoft.Data.SqlClient;

namespace DogGo.Repositories
{
    public class WalkRepository : BaseRepository, IWalkRepository
    {
        public WalkRepository(IConfiguration configuration) : base(configuration) { }

        public List<Walk> GetWalksForWalker(int walkerId)
        {
            using (var conn = Connection)
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT  ws.Id,
                            ws.[Date],
                            ws.Duration,
                            ws.WalkerId,
                            d.Id as DogId,
                            d.Name as DogName
                    FROM Walks ws
                    JOIN Dog d on ws.DogId = d.Id
                    WHERE ws.WalkerId = @id";

                    cmd.Parameters.AddWithValue("@id", walkerId);

                    var reader = cmd.ExecuteReader();

                    List<Walk> walks = new List<Walk>();

                    while (reader.Read())
                    {
                        Walk walk = new Walk
                        {
                            Id = DbUtils.GetInt(reader, "Id"),
                            Date = DbUtils.GetDateTime(reader, "Date"),
                            Duration = DbUtils.GetInt(reader, "Duration"),
                            WalkerId = DbUtils.GetInt(reader, "WalkerId"),
                            DogId = DbUtils.GetInt(reader, "DogId"),
                            DogName = DbUtils.GetString(reader, "DogName")
                        };
                        
                        walks.Add(walk);
                    }

                    reader.Close();

                    return walks;
                }
            }
        }
    }
}