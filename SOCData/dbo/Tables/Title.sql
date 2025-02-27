CREATE TABLE [dbo].[Title] (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NULL,
    Description NVARCHAR(1000) NULL,
    Author NVARCHAR(255) NULL,
    SpotId INT,
    FOREIGN KEY (SpotId) REFERENCES [dbo].[Spot](Id)
);