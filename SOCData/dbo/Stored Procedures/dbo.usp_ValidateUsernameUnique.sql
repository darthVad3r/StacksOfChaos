CREATE PROCEDURE [dbo].[usp_ValidateUsernameUnique]
    @Username NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) AS UserCount
    FROM Users
    WHERE UserName = @Username;
    IF @@ROWCOUNT = 0
    BEGIN
        RETURN 0;
    END
    RETURN 1;
END