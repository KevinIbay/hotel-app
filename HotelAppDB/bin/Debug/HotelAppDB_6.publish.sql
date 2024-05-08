﻿/*
Deployment script for HotelAppDB

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "HotelAppDB"
:setvar DefaultFilePrefix "HotelAppDB"
:setvar DefaultDataPath "C:\Users\Kevin\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\mssqllocaldb\"
:setvar DefaultLogPath "C:\Users\Kevin\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\mssqllocaldb\"

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
USE [$(DatabaseName)];


GO
PRINT N'Creating Procedure [dbo].[spRoomTypes_GetById]...';


GO
CREATE PROCEDURE [dbo].[spRoomTypes_GetById]
	@Id int
AS

BEGIN
	SET NOCOUNT ON;

	SELECT [Id], [Title], [Description], [Price]
	FROM dbo.RoomTypes
	WHERE Id = @Id;

END
GO
/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

IF NOT EXISTS (SELECT 1 FROM dbo.RoomTypes)
BEGIN
    INSERT INTO dbo.RoomTypes(Title, Description, Price)
    VALUES ('King Size Bed', 'A room with a king-size bed and a window', 100),
    ('Two Queen Size Bed', 'A room with two queen-size beds and a window', 115),
    ('Executive Suite', 'Two rooms, each with a king-size bed and a window', 205);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Rooms)
BEGIN
    DECLARE @roomId1 int;
    DECLARE @roomId2 int;
    DECLARE @roomId3 int;

    SELECT @roomId1 = Id FROM dbo.RoomTypes WHERE Title = 'King Size Bed';
    SELECT @roomId2 = Id FROM dbo.RoomTypes WHERE Title = 'Two Queen Size Bed';
    SELECT @roomId3 = Id FROM dbo.RoomTypes WHERE Title = 'Executive Suite';

    INSERT INTO dbo.Rooms(RoomNumber, RoomTypeId)
    VALUES ('101', @roomId1),
    ('102', @roomId1),
    ('103', @roomId1),
    ('201', @roomId2),
    ('202', @roomId2),
    ('301', @roomId3);
END
GO

GO
PRINT N'Update complete.';


GO
