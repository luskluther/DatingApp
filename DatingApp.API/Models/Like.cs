namespace DatingApp.API.Models
{
    public class Like // like class for liking someone or liked by someone else , sad trombone 
    {
        public int LikerId { get; set; }
        public int LikeeId { get; set; }
        public virtual User Liker { get; set; } // lazy loading
        public virtual User Likee { get; set; } // lazy loading
    }
}