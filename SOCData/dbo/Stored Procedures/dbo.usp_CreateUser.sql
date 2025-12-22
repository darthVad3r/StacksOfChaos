CREATE PROCEDURE [dbo].[usp_CreateUser]
    @Email NVARCHAR(320),
    @Password NVARCHAR(255) = NULL,
    @UserName NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Users (Email, CreatedDate, UserName, PasswordHash)
    VALUES (
        @Email, 
        GETDATE(), 
        ISNULL(@UserName, @Email), 
        CASE 
            WHEN @Password IS NOT NULL THEN HASHBYTES('SHA2_256', @Password)
            ELSE NULL 
        END
    );

    SELECT SCOPE_IDENTITY() AS UserID;
END