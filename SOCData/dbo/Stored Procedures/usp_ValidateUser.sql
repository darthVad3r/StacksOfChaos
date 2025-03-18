 CREATE PROCEDURE sp_ValidateUser
    @Username NVARCHAR(50),
    @PasswordHash NVARCHAR(255),
    @IsValid BIT OUTPUT
 AS
 BEGIN
    SET @IsValid = CASE WHEN EXISTS (
        SELECT 1 FROM Users 
        WHERE Username = @Username AND PasswordHash = @PasswordHash
    ) THEN 1 ELSE 0 END
 END