using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using BiancasBikeShop.Models;
using BiancasBikeShop.Utils;

namespace BiancasBikeShop.Repositories
{
    public class BikeRepository : IBikeRepository
    {
        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection("server=localhost\\SQLExpress;database=BiancasBikeShop;integrated security=true;TrustServerCertificate=true");
            }
        }

        public List<Bike> GetAllBikes()
        {
            var bikes = new List<Bike>();

            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
               SELECT b.Id, b.Brand, b.Color, 

                      ow.Id AS OwnerId, ow.Name, ow.Address, ow.Email, ow.Telephone,

                      bt.Id AS BikeTypeId, bt.Name AS TypeOfBike
                        
                 FROM Bike b 
                      JOIN Owner ow ON b.OwnerId = ow.Id
                      JOIN BikeType bt ON bt.Id = b.BikeTypeId
                 ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bikes.Add(new Bike()
                            {
                                Id = DbUtils.GetInt(reader, "Id"),
                                Brand = DbUtils.GetString(reader, "Brand"),
                                Color = DbUtils.GetString(reader, "Color"),
                                Owner = new Owner()
                                {
                                    Id = DbUtils.GetInt(reader, "OwnerId"),
                                    Name = DbUtils.GetString(reader, "Name"),
                                    Address = DbUtils.GetString(reader, "Address"),
                                    Email = DbUtils.GetString(reader, "Email"),
                                    Telephone = DbUtils.GetString(reader, "Telephone")
                                },
                                BikeType = new BikeType()
                                {
                                    Id = DbUtils.GetInt(reader, "BikeTypeId"),
                                    Name = DbUtils.GetString(reader, "TypeOfBike")
                                }
                            });
                        }

                        return bikes;
                    }
                }
            }
        }

        public Bike GetBikeById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                          SELECT b.Id, b.Brand, b.Color, 

                                 ow.Id AS OwnerId, ow.Name, ow.Address, ow.Email, ow.Telephone,

                                 bt.Id AS BikeTypeId, bt.Name AS TypeOfBike,

                                 wo.Id AS WorkOrderId, wo.DateInitiated, wo.Description,
                                 wo.DateCompleted, wo.BikeId

                          FROM Bike b 
                            JOIN Owner ow ON b.OwnerId = ow.Id
                            JOIN BikeType bt ON bt.Id = b.BikeTypeId
                            JOIN WorkOrder wo ON wo.BikeId = b.Id

                          WHERE b.Id = @Id";

                    DbUtils.AddParameter(cmd, "@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        Bike bike = null;
                        while (reader.Read())
                        {
                            if (bike == null)
                            {
                                bike = new Bike()
                                {
                                    Id = DbUtils.GetInt(reader, "Id"),
                                    Brand = DbUtils.GetString(reader, "Brand"),
                                    Color = DbUtils.GetString(reader, "Color"),
                                    Owner = new Owner()
                                    {
                                        Id = DbUtils.GetInt(reader, "OwnerId"),
                                        Name = DbUtils.GetString(reader, "Name"),
                                        Address = DbUtils.GetString(reader, "Address"),
                                        Email = DbUtils.GetString(reader, "Email"),
                                        Telephone = DbUtils.GetString(reader, "Telephone")
                                    },
                                    BikeType = new BikeType()
                                    {
                                        Id = DbUtils.GetInt(reader, "BikeTypeId"),
                                        Name = DbUtils.GetString(reader, "TypeOfBike")
                                    },
                                    WorkOrders = new List<WorkOrder>()
                                };

                                if (DbUtils.IsNotDbNull(reader, "WorkOrderId"))
                                {
                                    bike.WorkOrders.Add(new WorkOrder()
                                    {
                                        Id = DbUtils.GetInt(reader, "WorkOrderId"),
                                        DateInitiated = DbUtils.GetDateTime(reader, "DateInitiated"),
                                        Description = DbUtils.GetString(reader, "Description"),
                                        DateCompleted = DbUtils.GetNullableDateTime(reader, "DateCompleted")
                                    });
                                }
                            }                            
                        }

                        return bike;
                    }
                }
            }
        }

        public int GetBikesInShopCount()
        {
            int count = 0;

            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT COUNT(DISTINCT b.Id) as BikesInShop
                    FROM Bike b
                        LEFT JOIN WorkOrder wo ON wo.BikeId = b.Id
                    WHERE wo.DateCompleted IS null
                    ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            count = DbUtils.GetInt(reader, "BikesInShop");
                        }

                        return count;
                    }
                }
            }
        }
    }
}
