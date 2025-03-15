CREATE PROCEDURE [dbo].[usp_VerifyIfUserExists]
	@UserName nvarchar(50),
	@Email nvarchar(50)
	AS
BEGIN
	-- Check if a user with the given username or email already exists
    IF EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [UserName] = @UserName OR [Email] = @Email)
    BEGIN
        -- Return 1 if the user exists
        SELECT 1 AS UserExists
    END
    ELSE
    BEGIN
        -- Return 0 if the user does not exist
        SELECT 0 AS UserExists
    END
END