
using API.DTOs;
using API.Models;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Services
{
    public class UserService : IUserService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "Users";

        public UserService(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        public async Task<User> CreateAsync(RegisterDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            
            CollectionReference collectionRef = _firestoreDb.Collection(CollectionName);
            DocumentReference docRef = await collectionRef.AddAsync(user);
            user.Id = docRef.Id;
            return user;
        }
        public async Task<User> CreateAsync(UserCreateDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            var docRef = await _firestoreDb.Collection(CollectionName).AddAsync(user);
            user.Id = docRef.Id;
            return user;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot userRef = await docRef.GetSnapshotAsync();

            if(!userRef.Exists)
            {
                return false;
            }

            await docRef.DeleteAsync();
            return true;


        }

        public async Task<List<UserResponseDto>> GetAllAsync()
        {
            Query query = _firestoreDb.Collection(CollectionName);
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            return querySnapshot.Documents.Select(doc => new UserResponseDto
            {
                Id = doc.Id,
                Username = doc.GetValue<string>("Username"),
                Email = doc.GetValue<string>("Email")
            }).ToList();
        }

        public async Task<UserResponseDto?> GetByIdAsync(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot docSnapshot = await docRef.GetSnapshotAsync();
            if(!docSnapshot.Exists)
            {
                return null;
            }

            return new UserResponseDto
            {
                Id = docSnapshot.Id,
                Username = docSnapshot.GetValue<string>("Username"),
                Email = docSnapshot.GetValue<string>("Email")
            };

        }

        public async Task<UserResponseDto?> UpdateAsync(string id, UserUpdateDto dto)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot docSnapshot = await docRef.GetSnapshotAsync();
            if(!docSnapshot.Exists)
            {
                return null;
            }

            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "Username", dto.Username },
                { "Email", dto.Email }
            };

            await docRef.UpdateAsync(updates);
            return new UserResponseDto
            {
                Id = docSnapshot.Id,
                Username = dto.Username,
                Email = dto.Email
            };
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            Query query = _firestoreDb.Collection(CollectionName).WhereEqualTo("Email", email);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            DocumentSnapshot? userDoc = snapshot.Documents.FirstOrDefault();

            if (userDoc == null) return null;

            // Mapeando manualmente para o objeto User (usado no Login/Auth)
            return new User
            {
                Id = userDoc.Id,
                Username = userDoc.GetValue<string>("Username"),
                Email = userDoc.GetValue<string>("Email"),
                PasswordHash = userDoc.GetValue<string>("PasswordHash")
            };
        }
    }
}
