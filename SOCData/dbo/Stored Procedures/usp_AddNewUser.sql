CREATE PROCEDURE [dbo].[usp_AddNewUser]
	@UserName nvarchar(50),
	@Password nvarchar(50),
	@Email nvarchar(50)
	AS
BEGIN
	INSERT INTO [dbo].[Users] ([UserName], [Password], [Email])
	VALUES (@UserName, @Password, @Email)
END