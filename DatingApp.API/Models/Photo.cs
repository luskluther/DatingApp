using System;

namespace DatingApp.API.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        public User User { get; set; } // step 1
        public int UserId { get; set; } // step 2 by doing these 2 steps the relationsship beweteen user and photo will be a cascade delete
        // so in genereal if a user is deleted the photos also will be deleted
        // default way in EF is with FK , if user is deleted photos stay but the FK id will be replaced by null and is not ideal.
        
        // this property is the public id for the photo that is saved in the cloud
        public string PublicId { get; set; }
    }
}