﻿CREATE TABLE [dbo].[Bookings]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [StartDate] DATETIME2 NOT NULL, 
    [EndDate] DATETIME2 NOT NULL, 
    [CheckedIn] BIT NOT NULL DEFAULT 0, 
    [TotalCost] MONEY NOT NULL, 
    [RoomId] INT NOT NULL, 
    [GuestId] INT NOT NULL, 
    CONSTRAINT [FK_Bookings_Rooms] FOREIGN KEY (RoomId) REFERENCES Rooms(Id), 
    CONSTRAINT [FK_Bookings_Guests] FOREIGN KEY (GuestId) REFERENCES Guests(Id)
)