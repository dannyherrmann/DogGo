using DogGo.Models;
using DogGo.Utils;
using Microsoft.Data.SqlClient;

namespace DogGo.Repositories
{
    public class DogRepository : BaseRepository, IDogRepository
    {
        public DogRepository(IConfiguration configuration) : base(configuration) { }

        public List<Dog> GetAllDogs()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    select  d.Id as DogId, 
                            d.[Name] as DogName, 
                            d.Breed as DogBreed, 
                            d.Notes as DogNotes, 
                            d.ImageUrl as DogAvatar,
                            o.Id as OwnerId,
                            o.Name as OwnerName,
                            o.Email as OwnerEmail,
                            o.Address as OwnerAddress,
                            o.Phone as OwnerPhone,
                            o.NeighborhoodId as OwnerNeighborhoodId 
                    FROM Dog d
                    JOIN [Owner] o on d.OwnerId = o.Id";

                    var reader = cmd.ExecuteReader();

                    List<Dog> dogs = new List<Dog>();

                    while (reader.Read())
                    {
                        Dog dog = new Dog
                        {
                            Id = DbUtils.GetInt(reader, "DogId"),
                            Name = DbUtils.GetString(reader, "DogName"),
                            OwnerId = DbUtils.GetInt(reader, "OwnerId"),
                            Breed = DbUtils.GetString(reader, "DogBreed"),
                            Notes = DbUtils.GetString(reader, "DogNotes"),
                            ImageUrl = DbUtils.GetString(reader, "DogAvatar"),
                            Owner = new Owner
                            {
                                Id = DbUtils.GetInt(reader, "OwnerId"),
                                Name = DbUtils.GetString(reader, "OwnerName"),
                                Email = DbUtils.GetString(reader, "OwnerEmail"),
                                Address = DbUtils.GetString(reader, "OwnerAddress"),
                                Phone = DbUtils.GetString(reader, "OwnerPhone"),
                                NeighborhoodId = DbUtils.GetInt(reader, "OwnerNeighborhoodId")
                            }
                        };

                        dogs.Add(dog);
                    }

                    reader.Close();

                    return dogs;
                }
            }
        }

        public Dog GetDogById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    select  d.Id as DogId, 
                            d.[Name] as DogName, 
                            d.Breed as DogBreed, 
                            d.Notes as DogNotes, 
                            d.ImageUrl as DogAvatar,
                            o.Id as OwnerId,
                            o.Name as OwnerName,
                            o.Email as OwnerEmail,
                            o.Address as OwnerAddress,
                            o.Phone as OwnerPhone,
                            o.NeighborhoodId as OwnerNeighborhoodId 
                    FROM Dog d
                    JOIN [Owner] o on d.OwnerId = o.Id
                    WHERE d.Id = @Id";

                    DbUtils.AddParameter(cmd, "@Id", id);

                    var reader = cmd.ExecuteReader();

                    Dog dog = null;

                    while (reader.Read())
                    {
                        if (dog == null)
                        {
                            dog = new Dog()
                            {
                                Id = DbUtils.GetInt(reader, "DogId"),
                                Name = DbUtils.GetString(reader, "DogName"),
                                OwnerId = DbUtils.GetInt(reader, "OwnerId"),
                                Breed = DbUtils.GetString(reader, "DogBreed"),
                                Notes = DbUtils.GetString(reader, "DogNotes"),
                                ImageUrl = DbUtils.GetString(reader, "DogAvatar"),
                                Owner = new Owner
                                {
                                    Id = DbUtils.GetInt(reader, "OwnerId"),
                                    Name = DbUtils.GetString(reader, "OwnerName"),
                                    Email = DbUtils.GetString(reader, "OwnerEmail"),
                                    Address = DbUtils.GetString(reader, "OwnerAddress"),
                                    Phone = DbUtils.GetString(reader, "OwnerPhone"),
                                    NeighborhoodId = DbUtils.GetInt(reader, "OwnerNeighborhoodId")
                                }
                            };
                        }
                    }

                    reader.Close();

                    return dog;
                }
            }
        }

        public void AddDog(Dog dog)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO Dog ([Name], OwnerId, Breed, Notes, ImageUrl)
                    OUTPUT INSERTED.ID
                    VALUES (@name, @ownerId, @breed, @notes, @imageUrl);";

                    cmd.Parameters.AddWithValue("@name", dog.Name);
                    cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);
                    cmd.Parameters.AddWithValue("@breed", dog.Breed);
                    cmd.Parameters.AddWithValue("@notes", dog.Notes);
                    cmd.Parameters.AddWithValue("@imageUrl", dog.ImageUrl);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateDog(Dog dog)
        {
            using (var conn = Connection)
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    UPDATE Dog
                    SET 
                        [Name] = @name,
                        OwnerId = @ownerId,
                        Breed = @breed,
                        Notes = @notes,
                        ImageUrl = @imageUrl
                    WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@name", dog.Name);
                    cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);
                    cmd.Parameters.AddWithValue("@breed", dog.Breed);
                    cmd.Parameters.AddWithValue("@notes", dog.Notes);
                    cmd.Parameters.AddWithValue("@imageUrl", dog.ImageUrl);
                    cmd.Parameters.AddWithValue("@id", dog.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteDog(int dogId)
        {
            using (var conn = Connection)
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    DELETE FROM Dog
                    WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@id", dogId);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}