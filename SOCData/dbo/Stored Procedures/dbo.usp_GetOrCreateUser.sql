CREATE PROCEDURE [dbo].[usp_GetOrCreateUser]
	@Email NVARCHAR(320),
	@Password NVARCHAR(255) = NULL,
	@UserName NVARCHAR(255) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @UserID INT;
	SELECT @UserID = Id
	FROM Users
	WHERE Email = @Email;
	
	IF @UserID IS NULL
	BEGIN
		INSERT INTO Users (Email, CreatedDate, UserName)
		VALUES (@Email, @UserName, GETDATE(), @Email, 
			CASE 
				WHEN @Password IS NOT NULL THEN HASHBYTES('SHA2_256', @Password)
				ELSE NULL 
			END);
		SET @UserID = SCOPE_IDENTITY();
	END
	SELECT @UserID AS UserID, @Email as Email, @UserID as UserID, @UserName as UserName;
END
GO


