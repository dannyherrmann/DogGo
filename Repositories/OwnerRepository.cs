using DogGo.Models;
using DogGo.Utils;
using Microsoft.Data.SqlClient;

namespace DogGo.Repositories
{
    public class OwnerRepository : BaseRepository, IOwnerRepository
    {
        public OwnerRepository(IConfiguration configuration) : base(configuration) { }

        public List<Owner> GetAllOwners()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    select  Id, 
                            Email, 
                            [Name], 
                            Address, 
                            NeighborhoodId, 
                            Phone
                    FROM Owner";

                    var reader = cmd.ExecuteReader();

                    List<Owner> owners = new List<Owner>();
                    while (reader.Read())
                    {
                        Owner owner = new Owner
                        {
                            Id = DbUtils.GetInt(reader, "Id"),
                            Email = DbUtils.GetString(reader, "Email"),
                            Name = DbUtils.GetString(reader, "Name"),
                            Address = DbUtils.GetString(reader, "Address"),
                            NeighborhoodId = DbUtils.GetInt(reader, "NeighborhoodId"),
                            Phone = DbUtils.GetString(reader, "Phone")
                        };

                        owners.Add(owner);
                    }

                    reader.Close();

                    return owners;
                }
            }
        }

        public Owner GetOwnerById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    select  Id, 
                            Email, 
                            [Name], 
                            Address, 
                            NeighborhoodId, 
                            Phone
                    FROM Owner
                    WHERE Id = @Id";

                    DbUtils.AddParameter(cmd, "@Id", id);

                    var reader = cmd.ExecuteReader();

                    Owner owner = null;

                    if (reader.Read())
                    {
                        owner = new Owner()
                        {
                            Id = DbUtils.GetInt(reader, "Id"),
                            Email = DbUtils.GetString(reader, "Email"),
                            Name = DbUtils.GetString(reader, "Name"),
                            Address = DbUtils.GetString(reader, "Address"),
                            NeighborhoodId = DbUtils.GetInt(reader, "NeighborhoodId"),
                            Phone = DbUtils.GetString(reader, "Phone")
                        };
                    }

                    reader.Close();
                    return owner;
                }
            }
        }
    }
}