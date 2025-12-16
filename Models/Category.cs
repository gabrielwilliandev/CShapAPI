using Google.Cloud.Firestore;

namespace API.Models
{
    [FirestoreData]
    public class Category
    {
        [FirestoreProperty]
        public int Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public List<Transacao> Transactions { get; set; }
    }
}
