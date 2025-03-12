CREATE PROCEDURE [dbo].[usp_AddNewUser]
    @Name NVARCHAR(100),
    @Email NVARCHAR(255),
    @Password NVARCHAR(255),
    @Role NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;  -- Automatically rollback on errors
    
    -- Input validation
    IF @Name IS NULL OR LEN(TRIM(@Name)) = 0
        THROW 50000, 'Name is required.', 1;
    IF @Email NOT LIKE '%_@__%.__%' OR @Email LIKE '%[^a-zA-Z0-9.@_-]%'
        THROW 50000, 'Invalid email format.', 1;

    IF @Password NOT LIKE '%[0-9]%' OR @Password NOT LIKE '%[A-Z]%' OR @Password NOT LIKE '%[a-z]%'
        THROW 50000, 'Password must contain numbers, uppercase and lowercase letters.', 1;
    IF @Role IS NULL OR @Role NOT IN ('Admin', 'User')
        THROW 50000, 'Invalid role specified.', 1;

    DECLARE @CurrentTime DATETIME2(7) = SYSUTCDATETIME();  -- More precise than GETUTCDATE()

    BEGIN TRY
        BEGIN TRANSACTION;
            -- Check if email already exists (with index hint)
            IF EXISTS (
                SELECT 1 
                FROM [dbo].[Users] WITH (UPDLOCK, HOLDLOCK, INDEX(IX_Users_Email))
                WHERE [Email] = @Email 
                AND [IsDeleted] = 0
            )
            BEGIN
                THROW 50001, 'User with this email already exists.', 1;
            END

            INSERT INTO [dbo].[Users] (
                [Name],
                [Email],
                [Password],
                [Role],
                [CreatedAt],
                [UpdatedAt],
                [IsVerified],
                [IsActivated],
                [IsDeleted]
            )
            VALUES (
                TRIM(@Name),
                LOWER(TRIM(@Email)),  -- Normalize email
                @Password,            -- Ensure password is hashed before passing
                @Role,
                @CurrentTime,
                @CurrentTime,
                0,                    -- IsVerified
                0,                    -- IsActivated
                0                     -- IsDeleted
            );

            DECLARE @NewUserId INT = SCOPE_IDENTITY();
            
            -- Return user details for confirmation
            SELECT 
                [UserId] = @NewUserId,
                [Name] = TRIM(@Name),
                [Email] = LOWER(TRIM(@Email)),
                [Role] = @Role,
                [CreatedAt] = @CurrentTime;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE(),
                @ErrorSeverity INT = ERROR_SEVERITY(),
                @ErrorState INT = ERROR_STATE();

        -- Log error details (implement your logging mechanism)
        -- EXEC sp_LogError @ErrorMessage, @ErrorSeverity, @ErrorState;

        THROW;
    END CATCH;
END

-- Example usage:
-- EXEC sp_AddNewUser 
--    @Name = 'John Doe',
--    @Email = 'john.doe@example.com',
--    @Password = 'hashedPassword123',
--    @Role = 'User'

--Important security notes:

--Ensure passwords are hashed before passing them to this procedure

--Consider adding additional input validation as needed

--Might want to add transaction handling for more complex scenarios

--Consider adding additional security checks based on requirements

--The procedure can be extended with additional parameters if needed