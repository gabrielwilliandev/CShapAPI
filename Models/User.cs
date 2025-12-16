using Google.Cloud.Firestore;

namespace API.Models
{   
    [FirestoreData]
    public class User
    {
        [FirestoreDocumentId]
        public string Id { get; set; }
        [FirestoreProperty]
        public string? Username { get; set; }
        [FirestoreProperty]
        public string? Email { get; set; }
        [FirestoreProperty]
        public string? PasswordHash { get; set; }
        [FirestoreProperty]
        public string Role { get; set; } = "User";
        [FirestoreProperty]

        public List<Transacao>? Transactions { get; set; }
    }
}
