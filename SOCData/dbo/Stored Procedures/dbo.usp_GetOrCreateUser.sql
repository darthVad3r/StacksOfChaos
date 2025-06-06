﻿CREATE PROCEDURE [dbo].[usp_GetOrCreateUser]
	@Email NVARCHAR(320),
	@Name NVARCHAR(255)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @UserID INT;
	SELECT @UserID = Id
	FROM Users
	WHERE Email = @Email;
	
	IF @UserID IS NULL
	BEGIN
		INSERT INTO Users (Email, Name, CreatedDate, UserName)
		VALUES (@Email, @Name, GETDATE(), @Email);
		SET @UserID = SCOPE_IDENTITY();
	END
	SELECT @UserID AS UserID, @Email as Email, @UserID as UserID;
END
GO


