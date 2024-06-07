using HotelAppLibrary.Databases;
using HotelAppLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelAppLibrary.Data
{
    public class SqliteData : IDatabaseData
    {
        private const string connectionStringName = "SQLiteDb";
        private readonly ISqliteDataAccess _db;

        public SqliteData(ISqliteDataAccess db)
        {
            _db = db;
        }


        public void BookGuest(string firstName, string lastName, DateTime startDate, DateTime endDate, int roomTypeId)
        {
            string sqlQuery = @"SELECT 1 FROM Guests WHERE FirstName = @firstName AND LastName = @lastName";
            int results = _db.LoadData<dynamic, dynamic>(sqlQuery,
                                                    new { firstName, lastName },
                                                    connectionStringName).Count();

            if (results == 0)
            {
                sqlQuery = @"INSERT INTO Guests (FirstName, LastName)
			                VALUES (@firstName, @lastName);";

                _db.SaveData(sqlQuery, new { firstName, lastName }, connectionStringName);
            }

            sqlQuery = @"SELECT [Id], [FirstName], [LastName]
	                    FROM Guests
	                    WHERE FirstName = @firstName AND LastName = @lastName
                        LIMIT 1;";

            GuestModel guest = _db.LoadData<GuestModel, dynamic>(sqlQuery,
                                                     new { firstName, lastName },
                                                     connectionStringName).First();

            RoomTypeModel roomType = _db.LoadData<RoomTypeModel, dynamic>("SELECT * FROM RoomTypes WHERE Id = @Id",
                                                                          new { Id = roomTypeId },
                                                                          connectionStringName).First();

            TimeSpan timeStaying = endDate.Date.Subtract(startDate.Date);

            sqlQuery = @"SELECT r.*
	                    FROM Rooms r
	                    INNER JOIN RoomTypes t
		                    ON t.Id = r.RoomTypeId
	                    WHERE r.RoomTypeId = @roomTypeId 
		                    AND r.Id NOT IN 
		                    (
			                    SELECT b.RoomId
			                    FROM Bookings b
			                    WHERE (@startDate < b.StartDate AND @endDate > b.EndDate)
				                    OR (b.StartDate <= @endDate AND @endDate < b.EndDate)
				                    OR (b.StartDate <= @startDate AND @startDate < b.EndDate)
		                    );";

            List<RoomModel> availableRooms = _db.LoadData<RoomModel, dynamic>(sqlQuery,
                                                                              new { startDate, endDate, roomTypeId },
                                                                              connectionStringName);

            sqlQuery = @"INSERT INTO Bookings(RoomId, GuestId, StartDate, EndDate, TotalCost)
	                    VALUES (@roomId, @guestId, @startDate, @endDate, @totalCost);";

            _db.SaveData(
                sqlQuery,
                new
                {
                    roomId = availableRooms.First().Id,
                    guestId = guest.Id,
                    startDate = startDate,
                    endDate = endDate,
                    totalCost = timeStaying.Days * roomType.Price
                },
                connectionStringName);
        }

        public void CheckInGuest(int bookingId)
        {
            string sqlQuery = @"UPDATE Bookings
	                            SET CheckedIn = 1
	                            WHERE Id = @Id;";

            _db.SaveData(sqlQuery,
                new { Id = bookingId },
                connectionStringName);
        }

        public List<RoomTypeModel> GetAvailableRoomTypes(DateTime startDate, DateTime endDate)
        {
            string sqlQuery = @"SELECT t.Id, t.Title, t.Description, t.Price
	                            FROM Rooms r
	                            INNER JOIN RoomTypes t
		                            ON t.Id = r.RoomTypeId
	                            WHERE r.Id NOT IN (
		                            SELECT b.RoomId
		                            FROM Bookings b
		                            WHERE (@startDate < b.StartDate AND @endDate > b.EndDate)
			                            OR (b.StartDate <= @endDate AND @endDate < b.EndDate)
			                            OR (b.StartDate <= @startDate AND @startDate < b.EndDate)
	                            )
	                            GROUP BY t.Id, t.Title, t.Description, t.Price;";

            var output = _db.LoadData<RoomTypeModel, dynamic>(sqlQuery,
                                                 new { startDate, endDate },
                                                 connectionStringName);

            output.ForEach(x => x.Price = x.Price / 100);

            return output;
        }

        public RoomTypeModel GetRoomTypeById(int id)
        {
            string sqlQuery = @"SELECT [Id], [Title], [Description], [Price]
	                            FROM RoomTypes
	                            WHERE Id = @id;";

            return _db.LoadData<RoomTypeModel, dynamic>(sqlQuery,
                                                        new { id },
                                                        connectionStringName).FirstOrDefault();
        }

        public List<BookingFullModel> SearchBookings(string lastName)
        {
            string sqlQuery = @"SELECT [b].[Id], [b].[RoomId], [b].[GuestId], [b].[StartDate], [b].[EndDate], [b].[CheckedIn],
                                [b].[TotalCost], [g].[FirstName], [g].[LastName], [r].[RoomNumber], [r].[RoomTypeId], [rt].[Title],
                                [rt].[Description], [rt].[Price]
                            FROM Bookings b
                            INNER JOIN Guests g
	                            ON b.GuestId = g.Id
                            INNER JOIN Rooms r
	                            ON b.RoomId = r.Id
                            INNER JOIN RoomTypes rt
	                            ON r.RoomTypeId = rt.Id
                            WHERE b.StartDate = @startDate
	                            AND g.LastName = @lastName;";
            var output = _db.LoadData<BookingFullModel, dynamic>(sqlQuery,
                                                    new { lastName, startDate = DateTime.Now.Date },
                                                    connectionStringName);

            output.ForEach(x => {
                x.Price = x.Price / 100;
                x.TotalCost = x.TotalCost / 100;
            });


            return output;
        }
    }
}
