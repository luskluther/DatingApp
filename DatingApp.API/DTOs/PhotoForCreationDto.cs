using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API.DTOs
{
    public class PhotoForCreationDto
    {
        public string  Url { get; set; }
        public IFormFile File { get; set; } // sending the file to upload with this , this will also be the parameter for the file from postman
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicId { get; set; } // this iswhat we get after the photo is uploaded from cloudinary

        public PhotoForCreationDto()
        {
            DateAdded = DateTime.Now;
        }
    }
}