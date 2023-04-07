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
                    select  o.Id as OwnerId,
                            o.Email as OwnerEmail,
                            o.[Name] as OwnerName,
                            o.Address as OwnerAddress,
                            o.Phone as OwnerPhone,
                            n.Id as NeighborhoodId,
                            n.Name as NeighborhoodName
                    FROM Owner o
                    JOIN Neighborhood n on o.NeighborhoodId = n.Id";

                    var reader = cmd.ExecuteReader();

                    List<Owner> owners = new List<Owner>();
                    while (reader.Read())
                    {
                        Owner owner = new Owner
                        {
                            Id = DbUtils.GetInt(reader, "OwnerId"),
                            Email = DbUtils.GetString(reader, "OwnerEmail"),
                            Name = DbUtils.GetString(reader, "OwnerName"),
                            Address = DbUtils.GetString(reader, "OwnerAddress"),
                            Phone = DbUtils.GetString(reader, "OwnerPhone"),
                            Neighborhood = new Neighborhood 
                            {
                                Id = DbUtils.GetInt(reader, "NeighborhoodId"),
                                Name = DbUtils.GetString(reader, "NeighborhoodName")
                            }
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
                    select  o.Id as OwnerId,
                            o.Email as OwnerEmail,
                            o.[Name] as OwnerName,
                            o.Address as OwnerAddress,
                            o.Phone as OwnerPhone,
                            n.Id as NeighborhoodId,
                            n.Name as NeighborhoodName,
                            d.Id as DogId,
                            d.Name as DogName,
                            d.Breed as DogBreed,
                            d.ImageUrl as DogAvatar,
                            d.Notes as DogNotes
                    FROM Owner o
                    JOIN Dog d on o.Id = d.OwnerId
                    JOIN Neighborhood n on o.NeighborhoodId = n.Id
                    WHERE o.Id = @Id";

                    DbUtils.AddParameter(cmd, "@Id", id);

                    var reader = cmd.ExecuteReader();

                    Owner owner = null;

                    while (reader.Read())
                    {
                        if (owner == null)
                        {
                            owner = new Owner()
                            {
                                Id = DbUtils.GetInt(reader, "OwnerId"),
                                Email = DbUtils.GetString(reader, "OwnerEmail"),
                                Name = DbUtils.GetString(reader, "OwnerName"),
                                Address = DbUtils.GetString(reader, "OwnerAddress"),
                                Phone = DbUtils.GetString(reader, "OwnerPhone"),
                                NeighborhoodId = DbUtils.GetInt(reader, "NeighborhoodId"),
                                Dogs = new List<Dog>(),
                                Neighborhood = new Neighborhood
                                {
                                    Id = DbUtils.GetInt(reader, "NeighborhoodId"),
                                    Name = DbUtils.GetString(reader, "NeighborhoodName")
                                }
                            };
                        }
                        
                        int dogId = DbUtils.GetInt(reader, "DogId");
                        if (dogId != null)
                        {
                            owner.Dogs.Add(new Dog()
                            {
                                Id = dogId,
                                Name = DbUtils.GetString(reader, "DogName"),
                                Breed = DbUtils.GetString(reader, "DogBreed"),
                                ImageUrl = DbUtils.GetString(reader, "DogAvatar"),
                                OwnerId = DbUtils.GetInt(reader, "OwnerId"),
                                Notes = DbUtils.GetString(reader, "DogNotes")
                            });
                        }
                    }

                    reader.Close();
                    return owner;
                }
            }
        }

        public Owner GetOwnerByEmail(string email)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    select  o.Id as OwnerId,
                            o.Email as OwnerEmail,
                            o.[Name] as OwnerName,
                            o.Address as OwnerAddress,
                            o.Phone as OwnerPhone,
                            n.Id as NeighborhoodId,
                            n.Name as NeighborhoodName,
                        FROM Owner o
                        JOIN Neighborhood n on o.NeighborhoodId = n.Id
                        WHERE o.Email = @email";

                    DbUtils.AddParameter(cmd, "@email", email);

                    var reader = cmd.ExecuteReader();

                    Owner owner = null;

                    if (reader.Read())
                    {
                        owner = new Owner()
                        {
                            Id = DbUtils.GetInt(reader, "OwnerId"),
                            Email = DbUtils.GetString(reader, "OwnerEmail"),
                            Name = DbUtils.GetString(reader, "OwnerName"),
                            Address = DbUtils.GetString(reader, "OwnerAddress"),
                            Phone = DbUtils.GetString(reader, "OwnerPhone"),
                            Dogs = new List<Dog>(),
                            Neighborhood = new Neighborhood
                            {
                                Id = DbUtils.GetInt(reader, "NeighborhoodId"),
                                Name = DbUtils.GetString(reader, "NeighborhoodName")
                            }
                        };
                    }

                    reader.Close();
                    return owner;
                }
            }
        }

        public void AddOwner(Owner owner)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO Owner ([Name], Email, Phone, Address, NeighborhoodId)
                    OUTPUT INSERTED.ID
                    VALUES (@name, @email, @phoneNumber, @address, @neighborhoodId);";

                    cmd.Parameters.AddWithValue("@name", owner.Name);
                    cmd.Parameters.AddWithValue("@email", owner.Email);
                    cmd.Parameters.AddWithValue("@phoneNumber", owner.Phone);
                    cmd.Parameters.AddWithValue("@address", owner.Address);
                    cmd.Parameters.AddWithValue("@neighborhoodId", owner.NeighborhoodId);

                    int id = (int)cmd.ExecuteScalar();

                    owner.Id = id;
                }
            }
        }

        public void UpdateOwner(Owner owner)
        {
            using (var conn = Connection)
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            UPDATE Owner
                            SET 
                                [Name] = @name, 
                                Email = @email, 
                                Address = @address, 
                                Phone = @phone, 
                                NeighborhoodId = @neighborhoodId
                            WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@name", owner.Name);
                    cmd.Parameters.AddWithValue("@email", owner.Email);
                    cmd.Parameters.AddWithValue("@address", owner.Address);
                    cmd.Parameters.AddWithValue("@phone", owner.Phone);
                    cmd.Parameters.AddWithValue("@neighborhoodId", owner.NeighborhoodId);
                    cmd.Parameters.AddWithValue("@id", owner.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteOwner(int ownerId)
        {
            using (var conn = Connection)
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            DELETE FROM Owner
                            WHERE Id = @id
                        ";

                    cmd.Parameters.AddWithValue("@id", ownerId);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}