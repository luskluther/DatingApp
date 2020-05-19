using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Route("api/users/{userId}/photos")]
    [ApiController]
    [Authorize]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepo _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly Cloudinary _cloudinary;
        public PhotosController(IDatingRepo repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this._cloudinaryConfig = cloudinaryConfig;
            this._mapper = mapper;
            this._repo = repo;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            ); // creating a cloudinary account

            _cloudinary = new Cloudinary(acc);
        }

        // carete as many custom dtos as possible for returning , we do not want to return any extra shit
        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id){
            var photoFromRepo = await _repo.GetPhoto(id);

            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }


        // what is happening here ?
        // 1. the local file is uploaded to our api first
        // 2. then it will be uploaded to cloudinary
        // 3. which will send a response
        // 4. then we are having our various mapping
        // 5. then we are retunring a createdatroute resposne which will send a 201
        // If there is a random error or model state error give [Frombody]/[Fromform] depending on request
        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreationDto photoForCreationDto) {

            // authorization
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)){
                return Unauthorized();
            }

            // getting the user
            var userFromRepo = await _repo.GetUser(userId);

            // whatever the file we are trying to upload
            var file = photoForCreationDto.File;

            // clouindary methods
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0) { // file is not empty
                using (var stream = file.OpenReadStream()) { // using since we are using memory and can be disposed that is the file that is in memry
                    // we are also transforming the photo for example if we send big photo
                    // we are transforming the photo to crop to small one and then cropping around the face
                    var uploadParams = new ImageUploadParams() {
                        File = new FileDescription(file.Name , stream), // combing the filename and file stream for upload params 
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    // we are uploading here and saving the result
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;
            // IMapper PhotosController.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);

            // if user doesnt have photos we can set it as main photo
            if (!userFromRepo.Photos.Any(userFromRepo => userFromRepo.IsMain)) {
                photo.IsMain = true;
            }

            userFromRepo.Photos.Add(photo);

            if (await _repo.SaveAll()) {

                // we need to return that particular photo id so that only happens after save
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
            
                // we are wrapping request qwith 201 and some other stuff as this reponse , GetPhoto in this case
                // the parameters for caretaedatroute are name of route , parameters dont know why wee are adding userid and object
                return CreatedAtRoute(nameof(GetPhoto), new { userId = userId, id = photo.Id }, photoToReturn);
            }
            return BadRequest("couldnt add the photo");
        }
        
        // what its doing is getting the users current main photo setting it to false and setting new photo to main
        [HttpPost("{id}/setMain")] // typically simple updates for restful apis are of type put or patch but post is conveniet
        public async Task<IActionResult> SetMainPhoto(int userId, int id) { 

            // authorization
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)){
                return Unauthorized();
            }

            // getting the user
            var userFromRepo = await _repo.GetUser(userId);

            if (!userFromRepo.Photos.Any(p => p.Id == id)) { // we can only update the photo if he has that photo 
                return Unauthorized();
            }

            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo.IsMain) {
                return BadRequest("This is already the main photo");
            }

            var currentMainPhoto = await _repo.GetMainPhoto(userId);

            currentMainPhoto.IsMain = false;
            photoFromRepo.IsMain = true;

            if(await _repo.SaveAll()) { 
                return NoContent();
            }

            return BadRequest("Unable to set photo to main");
        }

        // deleting should happen both in the cloud cloudinary as well as local DB
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id) { 
            // authorization
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)){
                return Unauthorized();
            }

            // getting the user
            var userFromRepo = await _repo.GetUser(userId);

            if (!userFromRepo.Photos.Any(p => p.Id == id)) { // we can only update the photo if he has that photo 
                return Unauthorized();
            }

            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo.IsMain) {
                return BadRequest("You cannot delete your main photo");
            }

            // if we have photo in cloud and db if statmement will exec otherwise else
            if (photoFromRepo.PublicId !=null) {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);

                var result = _cloudinary.Destroy(deleteParams);

                if (result.Result == "ok") {
                    _repo.Delete(photoFromRepo);
                }
            } else {
                _repo.Delete(photoFromRepo);
            }


            if (await _repo.SaveAll()) 
            return Ok();

            return BadRequest("Failed to delete the photo");
        }
    }
}