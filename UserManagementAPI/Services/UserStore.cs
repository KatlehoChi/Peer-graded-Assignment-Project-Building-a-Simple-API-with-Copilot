using System.Collections.Concurrent;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services;

public sealed class UserStore
{
    private const string DuplicateEmailMessage = "A user with this email already exists.";
    private readonly object mutationLock = new();

    private readonly ConcurrentDictionary<int, User> users = new(new[]
    {
        new KeyValuePair<int, User>(1, new User(1, "Avery", "Morgan", "avery.morgan@techhive.example", "Human Resources", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow)),
        new KeyValuePair<int, User>(2, new User(2, "Jordan", "Lee", "jordan.lee@techhive.example", "Information Technology", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow))
    });

    private int nextId = 2;

    public IReadOnlyList<User> GetAll() => users.Values.OrderBy(user => user.Id).ToArray();

    public User? GetById(int id) => users.GetValueOrDefault(id);

    public User Add(CreateUserRequest request)
    {
        lock (mutationLock)
        {
            var email = request.Email.Trim();
            EnsureEmailIsAvailable(email, null);

            var now = DateTimeOffset.UtcNow;
            var id = Interlocked.Increment(ref nextId);
            var user = new User(id, request.FirstName.Trim(), request.LastName.Trim(), email, request.Department.Trim(), now, now);
            users[id] = user;
            return user;
        }
    }

    public User? Update(int id, UpdateUserRequest request)
    {
        lock (mutationLock)
        {
            if (!users.TryGetValue(id, out var existing))
            {
                return null;
            }

            var email = request.Email.Trim();
            EnsureEmailIsAvailable(email, id);
            var updated = existing with
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = email,
                Department = request.Department.Trim(),
                UpdatedAt = DateTimeOffset.UtcNow
            };

            users[id] = updated;
            return updated;
        }
    }

    public bool Delete(int id)
    {
        lock (mutationLock)
        {
            return users.TryRemove(id, out _);
        }
    }

    private void EnsureEmailIsAvailable(string email, int? currentUserId)
    {
        if (users.Values.Any(user => user.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && user.Id != currentUserId))
        {
            throw new DuplicateEmailException(DuplicateEmailMessage);
        }
    }
}

public sealed class DuplicateEmailException(string message) : Exception(message);
