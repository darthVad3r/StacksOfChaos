CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    UserName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Token NVARCHAR(MAX),
    Role NVARCHAR(50),
    RefreshToken NVARCHAR(MAX),
    RefreshTokenExpiryTime DATETIME,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    IsVerified BIT NOT NULL DEFAULT 0,
    IsLocked BIT NOT NULL DEFAULT 0,
    IsActivated BIT NOT NULL DEFAULT 0,
    IsLoggedIn BIT NOT NULL DEFAULT 0,
    IsLoggedOut BIT NOT NULL DEFAULT 0,
    IsOnline BIT NOT NULL DEFAULT 0,
    IsOffline BIT NOT NULL DEFAULT 0,
    IsBlocked BIT NOT NULL DEFAULT 0,
    IsUnblocked BIT NOT NULL DEFAULT 0,
    IsSuspended BIT NOT NULL DEFAULT 0,
    IsUnsuspended BIT NOT NULL DEFAULT 0
);
GO

-- Create index on Email for faster lookups
CREATE UNIQUE INDEX IX_Users_Email ON Users(Email);

