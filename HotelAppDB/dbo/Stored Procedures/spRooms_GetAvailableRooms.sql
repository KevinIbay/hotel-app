CREATE PROCEDURE [dbo].[spRooms_GetAvailableRooms]
	@startDate date,
	@endDate date,
	@roomTypeId int
AS

BEGIN
	SET NOCOUNT ON;

	SELECT r.*
	FROM dbo.Rooms r
	INNER JOIN dbo.RoomTypes t
		ON t.Id = r.RoomTypeId
	WHERE r.RoomTypeId = @roomTypeId 
		AND r.Id NOT IN 
		(
			SELECT b.RoomId
			FROM dbo.Bookings b
			WHERE (@startDate < b.StartDate AND @endDate > b.EndDate)
				OR (b.StartDate <= @endDate AND @endDate < b.EndDate)
				OR (b.StartDate <= @startDate AND @startDate < b.EndDate)
		);
END
