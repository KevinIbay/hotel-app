CREATE PROCEDURE [dbo].[spRoomTypes_GetAvailableTypes]
	@startDate date,
	@endDate date
AS

BEGIN
	SET NOCOUNT ON;

	SELECT t.Id, t.Title, t.Description, t.Price
	FROM dbo.Rooms r
	INNER JOIN dbo.RoomTypes t
		ON t.Id = r.RoomTypeId
	WHERE r.Id NOT IN (
		SELECT b.RoomId
		FROM dbo.Bookings b
		WHERE (@startDate < b.StartDate AND @endDate > b.EndDate)
			OR (b.StartDate <= @endDate AND @endDate < b.EndDate)
			OR (b.StartDate <= @startDate AND @startDate < b.EndDate)
	)
	GROUP BY t.Id, t.Title, t.Description, t.Price;

END
