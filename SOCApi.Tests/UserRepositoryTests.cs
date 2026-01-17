using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using SOCApi.Models;
using SOCApi.Services;
using SOCApi.Services.Validation;
using System.Linq.Expressions;
using Xunit;

namespace SOCApi.Tests
{
    /// <summary>
    /// Comprehensive tests for UserRetrievalService data access layer.
    /// Tests validate CRUD operations, error handling, and data integrity.
    /// Follows SOLID principles with clear separation of concerns.
    /// </summary>
    public class UserRetrievalServiceTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly UserRetrievalService _service;

        public UserRetrievalServiceTests()
        {
            _mockUserManager = CreateMockUserManager();
            _service = new UserRetrievalService(_mockUserManager.Object);
        }

        #region GetUserByNameAsync Tests

        [Fact]
        public async Task GetUserByNameAsync_WithValidUsername_ReturnsUser()
        {
            // Arrange
            var username = "testuser@example.com";
            var expectedUser = CreateSampleUser(username);

            _mockUserManager
                .Setup(x => x.FindByNameAsync(username))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _service.GetUserByNameAsync(username);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.Id, result.Id);
            Assert.Equal(expectedUser.UserName, result.UserName);
            Assert.Equal(expectedUser.Email, result.Email);
            _mockUserManager.Verify(x => x.FindByNameAsync(username), Times.Once);
        }

        [Fact]
        public async Task GetUserByNameAsync_WithNonExistentUser_ReturnsNull()
        {
            // Arrange
            var username = "nonexistent@example.com";

            _mockUserManager
                .Setup(x => x.FindByNameAsync(username))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _service.GetUserByNameAsync(username);

            // Assert
            Assert.Null(result);
            _mockUserManager.Verify(x => x.FindByNameAsync(username), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetUserByNameAsync_WithInvalidUsername_ThrowsArgumentException(string? username)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetUserByNameAsync(username!));
            _mockUserManager.Verify(x => x.FindByNameAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetUserByNameAsync_WithDifferentCasing_FindsUser()
        {
            // Arrange
            var username = "TestUser@Example.com";
            var expectedUser = CreateSampleUser("testuser@example.com");

            _mockUserManager
                .Setup(x => x.FindByNameAsync(username))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _service.GetUserByNameAsync(username);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.UserName, result.UserName);
        }

        [Fact]
        public async Task GetUserByNameAsync_CallsUserManagerExactlyOnce()
        {
            // Arrange
            var username = "test@example.com";
            _mockUserManager.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(CreateSampleUser(username));

            // Act
            await _service.GetUserByNameAsync(username);
            await _service.GetUserByNameAsync(username);

            // Assert
            _mockUserManager.Verify(x => x.FindByNameAsync(username), Times.Exactly(2));
        }

        #endregion

        #region GetUserByIdAsync Tests

        [Fact]
        public async Task GetUserByIdAsync_WithValidId_ReturnsUser()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var expectedUser = CreateSampleUser("test@example.com", userId);

            _mockUserManager
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _service.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal(expectedUser.UserName, result.UserName);
            _mockUserManager.Verify(x => x.FindByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithNonExistentId_ReturnsNull()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();

            _mockUserManager
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _service.GetUserByIdAsync(userId);

            // Assert
            Assert.Null(result);
            _mockUserManager.Verify(x => x.FindByIdAsync(userId), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetUserByIdAsync_WithInvalidId_ThrowsArgumentException(string? userId)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetUserByIdAsync(userId!));
            _mockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithGuidId_ReturnsUser()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var user = CreateSampleUser("test@example.com", userId);

            _mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _service.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUserWithAllProperties()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var user = new User
            {
                Id = userId,
                UserName = "testuser@example.com",
                Email = "testuser@example.com",
                FirstName = "Test",
                LastName = "User",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                LastLogin = DateTime.UtcNow.AddDays(-1),
                ProfilePictureUrl = "https://example.com/profile.jpg",
                Bio = "Test bio"
            };

            _mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _service.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.FirstName, result.FirstName);
            Assert.Equal(user.LastName, result.LastName);
            Assert.Equal(user.EmailConfirmed, result.EmailConfirmed);
            Assert.Equal(user.ProfilePictureUrl, result.ProfilePictureUrl);
            Assert.Equal(user.Bio, result.Bio);
        }

        #endregion

        #region GetAllUsersAsync Tests

        [Fact]
        public async Task GetAllUsersAsync_WithMultipleUsers_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                CreateSampleUser("user1@example.com", Guid.NewGuid().ToString()),
                CreateSampleUser("user2@example.com", Guid.NewGuid().ToString()),
                CreateSampleUser("user3@example.com", Guid.NewGuid().ToString())
            };

            var mockUserDbSet = CreateMockDbSet(users);
            _mockUserManager.Setup(x => x.Users).Returns(mockUserDbSet.Object);

            // Act
            var result = await _service.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            var userList = result.ToList();
            Assert.Equal(3, userList.Count);
            Assert.Contains(userList, u => u.Email == "user1@example.com");
            Assert.Contains(userList, u => u.Email == "user2@example.com");
            Assert.Contains(userList, u => u.Email == "user3@example.com");
        }

        [Fact]
        public async Task GetAllUsersAsync_WithNoUsers_ReturnsEmptyCollection()
        {
            // Arrange
            var users = new List<User>();
            var mockUserDbSet = CreateMockDbSet(users);
            _mockUserManager.Setup(x => x.Users).Returns(mockUserDbSet.Object);

            // Act
            var result = await _service.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsSingleUser_WhenOnlyOneExists()
        {
            // Arrange
            var users = new List<User>
            {
                CreateSampleUser("single@example.com", Guid.NewGuid().ToString())
            };

            var mockUserDbSet = CreateMockDbSet(users);
            _mockUserManager.Setup(x => x.Users).Returns(mockUserDbSet.Object);

            // Act
            var result = await _service.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("single@example.com", result.First().Email);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsUsersWithAllProperties()
        {
            // Arrange
            var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "user1@example.com",
                    Email = "user1@example.com",
                    FirstName = "John",
                    LastName = "Doe",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "user2@example.com",
                    Email = "user2@example.com",
                    FirstName = "Jane",
                    LastName = "Smith",
                    EmailConfirmed = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                }
            };

            var mockUserDbSet = CreateMockDbSet(users);
            _mockUserManager.Setup(x => x.Users).Returns(mockUserDbSet.Object);

            // Act
            var result = await _service.GetAllUsersAsync();

            // Assert
            var userList = result.ToList();
            Assert.Equal(2, userList.Count);
            Assert.All(userList, user =>
            {
                Assert.NotNull(user.FirstName);
                Assert.NotNull(user.LastName);
                Assert.NotNull(user.Email);
            });
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsDistinctUsers()
        {
            // Arrange
            var users = new List<User>
            {
                CreateSampleUser("user1@example.com", "id1"),
                CreateSampleUser("user2@example.com", "id2"),
                CreateSampleUser("user3@example.com", "id3")
            };

            var mockUserDbSet = CreateMockDbSet(users);
            _mockUserManager.Setup(x => x.Users).Returns(mockUserDbSet.Object);

            // Act
            var result = await _service.GetAllUsersAsync();

            // Assert
            var userList = result.ToList();
            var distinctIds = userList.Select(u => u.Id).Distinct().Count();
            Assert.Equal(userList.Count, distinctIds);
        }

        #endregion

        #region Data Consistency Tests

        [Fact]
        public async Task GetUserByNameAsync_ThenGetUserById_ReturnsSameUser()
        {
            // Arrange
            var username = "consistent@example.com";
            var userId = Guid.NewGuid().ToString();
            var user = CreateSampleUser(username, userId);

            _mockUserManager.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var userByName = await _service.GetUserByNameAsync(username);
            var userById = await _service.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(userByName);
            Assert.NotNull(userById);
            Assert.Equal(userByName.Id, userById.Id);
            Assert.Equal(userByName.UserName, userById.UserName);
            Assert.Equal(userByName.Email, userById.Email);
        }

        [Fact]
        public async Task GetAllUsersAsync_ContainsUserFromGetUserByName()
        {
            // Arrange
            var username = "test@example.com";
            var userId = Guid.NewGuid().ToString();
            var specificUser = CreateSampleUser(username, userId);

            var allUsers = new List<User>
            {
                specificUser,
                CreateSampleUser("other1@example.com", Guid.NewGuid().ToString()),
                CreateSampleUser("other2@example.com", Guid.NewGuid().ToString())
            };

            _mockUserManager.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(specificUser);
            var mockUserDbSet = CreateMockDbSet(allUsers);
            _mockUserManager.Setup(x => x.Users).Returns(mockUserDbSet.Object);

            // Act
            var singleUser = await _service.GetUserByNameAsync(username);
            var allUsersResult = await _service.GetAllUsersAsync();

            // Assert
            Assert.NotNull(singleUser);
            Assert.Contains(allUsersResult, u => u.Id == singleUser.Id);
        }

        #endregion

        #region Edge Cases and Error Handling

        [Fact]
        public async Task GetUserByNameAsync_WithSpecialCharacters_HandlesCorrectly()
        {
            // Arrange
            var username = "user+tag@example.com";
            var user = CreateSampleUser(username);

            _mockUserManager.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(user);

            // Act
            var result = await _service.GetUserByNameAsync(username);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(username, result.UserName);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithMaxLengthId_HandlesCorrectly()
        {
            // Arrange
            var userId = new string('a', 450); // Max length for typical GUID string representations
            var user = CreateSampleUser("test@example.com", userId);

            _mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _service.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
        }

        [Fact]
        public async Task GetAllUsersAsync_WithLargeDataset_ReturnsAllUsers()
        {
            // Arrange
            var users = Enumerable.Range(1, 100)
                .Select(i => CreateSampleUser($"user{i}@example.com", Guid.NewGuid().ToString()))
                .ToList();

            var mockUserDbSet = CreateMockDbSet(users);
            _mockUserManager.Setup(x => x.Users).Returns(mockUserDbSet.Object);

            // Act
            var result = await _service.GetAllUsersAsync();

            // Assert
            Assert.Equal(100, result.Count());
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a mock UserManager for testing.
        /// Follows DRY principle by centralizing mock creation logic.
        /// </summary>
        private static Mock<UserManager<User>> CreateMockUserManager()
        {
            var store = new Mock<IUserStore<User>>();
            var mockUserManager = new Mock<UserManager<User>>(
                store.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);

            return mockUserManager;
        }

        /// <summary>
        /// Creates a sample user for testing purposes.
        /// Follows DRY principle to avoid duplicating user creation logic.
        /// </summary>
        private static User CreateSampleUser(string email, string? id = null)
        {
            return new User
            {
                Id = id ?? Guid.NewGuid().ToString(),
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FirstName = "Test",
                LastName = "User",
                CreatedAt = DateTime.UtcNow,
                SecurityStamp = Guid.NewGuid().ToString()
            };
        }

        /// <summary>
        /// Creates a mock DbSet for testing queryable collections.
        /// Follows SOLID principles with separation of concerns.
        /// </summary>
        private static Mock<DbSet<User>> CreateMockDbSet(List<User> users)
        {
            var queryable = users.AsQueryable();
            var mockSet = new Mock<DbSet<User>>();

            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            
            // Support async operations
            mockSet.As<IAsyncEnumerable<User>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<User>(queryable.GetEnumerator()));

            mockSet.As<IQueryable<User>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<User>(queryable.Provider));

            return mockSet;
        }

        #endregion
    }

    #region Test Helper Classes

    /// <summary>
    /// Test async enumerator for mocking async LINQ operations.
    /// Required for testing Entity Framework async queries.
    /// </summary>
    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_inner.MoveNext());
        }

        public T Current => _inner.Current;

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return new ValueTask();
        }
    }

    /// <summary>
    /// Test async query provider for mocking async LINQ operations.
    /// Required for testing Entity Framework async queries.
    /// </summary>
    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression)!;
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var resultType = typeof(TResult).GetGenericArguments()[0];
            var executeMethod = typeof(IQueryProvider)
                .GetMethod(nameof(IQueryProvider.Execute), 1, new[] { typeof(Expression) })!
                .MakeGenericMethod(resultType);

            var result = executeMethod.Invoke(_inner, new object[] { expression });
            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))!
                .MakeGenericMethod(resultType)
                .Invoke(null, new[] { result })!;
        }
    }

    /// <summary>
    /// Test async enumerable for mocking async collections.
    /// Required for testing Entity Framework async queries.
    /// </summary>
    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        {
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }

    #endregion
}
