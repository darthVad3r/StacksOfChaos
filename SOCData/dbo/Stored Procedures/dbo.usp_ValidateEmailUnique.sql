CREATE PROCEDURE dbo.usp_ValidateEmailUnique
    @Email NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserCount INT;

    SELECT @UserCount = COUNT(*)
    FROM Users
    WHERE Email = @Email;

    RETURN @UserCount;
END