using Google.Cloud.Firestore;

namespace API.Services
{
    public class FireStoreService
    {
        private readonly FirestoreDb _firestoreDb;

        public FireStoreService(IConfiguration configuration)
        {
            var projectId = configuration["FireBase:ProjectId"];
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        public FirestoreDb GetFirestoreDb()
        {
            return _firestoreDb;
        }
    }
}
