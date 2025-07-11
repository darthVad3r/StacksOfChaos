CREATE PROCEDURE [dbo].[usp_GetUsersByEmail]
    @Email NVARCHAR(320)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id AS Id, Email, UserName
    FROM Users
    WHERE Email = @Email;
END