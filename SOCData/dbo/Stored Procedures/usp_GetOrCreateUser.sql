CREATE PROCEDURE usp_GetOrCreateUser
	@Email NVARCHAR(255),
	@Name NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @UserID INT;
	SELECT @UserID = Id
	FROM Users
	WHERE Email = @Email;
	
	IF @UserID IS NULL
	BEGIN
		INSERT INTO Users (Email, Name, CreatedDate)
		VALUES (@Email, @Name, GETDATE());
		SET @UserID = SCOPE_IDENTITY();
	END
	SELECT @UserID;
END