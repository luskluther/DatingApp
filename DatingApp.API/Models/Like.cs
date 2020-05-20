namespace DatingApp.API.Models
{
    public class Like // like class for liking someone or liked by someone else , sad trombone 
    {
        public int LikerId { get; set; }
        public int LikeeId { get; set; }
        public User Liker { get; set; }
        public User Likee { get; set; }
    }
}