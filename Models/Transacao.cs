using Google.Cloud.Firestore;

namespace API.Models
{
    [FirestoreData]
    public class Transacao
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string? Description { get; set; }

        [FirestoreProperty]
        public decimal Amount { get; set; }

        [FirestoreProperty]
        public DateTime Date { get; set; }

        [FirestoreProperty]
        public string? Type { get; set; }

        [FirestoreProperty]
        public string CategoryId { get; set; } // Referência por string

        [FirestoreProperty]
        public string UserId { get; set; } // UID do Firebase Auth
    }
}