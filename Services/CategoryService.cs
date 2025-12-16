
using API.DTOs;
using API.Models;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "Categories";
        public CategoryService(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        public async Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto)
        {
            var category = new Category
            {
                Name = dto.Name
            };

            CollectionReference collectionRef = _firestoreDb.Collection(CollectionName);


            DocumentReference docRef = await collectionRef.AddAsync(new
            {
                Name = category.Name
            });

            return new CategoryResponseDto
            {
                Id = docRef.Id,
                Name = category.Name
            };
        }


        public async Task<bool> DeleteAsync(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot categoryRef = await docRef.GetSnapshotAsync();

            if (!categoryRef.Exists)
            {
                return false;
            }

            await docRef.DeleteAsync();

            return true;
        }

        public async Task<List<CategoryResponseDto>> GetAllAsync()
        {
            Query query = _firestoreDb.Collection(CollectionName);
            QuerySnapshot snapshot= await query.GetSnapshotAsync();

            return snapshot.Documents.Select(doc => new CategoryResponseDto
            {
                Id = doc.Id,
                Name = doc.GetValue<string>("Name")
            }).ToList();
        }

        public async Task<CategoryResponseDto?> GetByIdAsync(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();
            if (!docSnap.Exists)
            {
                return null;
            }
            return new CategoryResponseDto
            {
                Id = docSnap.Id,
                Name = docSnap.GetValue<string>("Name")
            };
        }

        public async Task<CategoryResponseDto?> UpdateAsync(string id, CategoryUpdateDto dto)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();
            if(!docSnap.Exists)
            {
                return null;
            }
            
            await docRef.UpdateAsync(new Dictionary<string, object>
            {
                { "Name", dto.Name }
            });
            return new CategoryResponseDto
            {
                Id = docSnap.Id,
                Name = dto.Name
            };
        }
    }
}
