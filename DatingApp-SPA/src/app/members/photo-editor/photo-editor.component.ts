import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Photo } from 'src/app/_models/photo';
import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {

  @Input() photos: Photo[];
  @Output() getMemberPhotoChanged = new EventEmitter<string>();
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  response: '';
  baseUrl = environment.apiUrl;
  currentMain: Photo;

  constructor(private _auth: AuthService, private _user: UserService, private _alert: AlertifyService) { }

  ngOnInit() {
    this.initializeUploader();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }
  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this._auth.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024 // this is nothing but max 10 mb
    });
    // a random fix for a random issue that you see in the console if not for this.
    this.uploader.onAfterAddingFile = (file) => { file.withCredentials = false; };

    this.uploader.onSuccessItem = (item, response, status, headers) => { // after upload success this is called
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded : res.dateAdded,
          description: res.description,
          isMain: res.isMain
        };
        this.photos.push(photo);
      }
    };
  }

  setMainPhoto(photo: Photo) {
    this._user.setMainPhoto(this._auth.decodedToken.nameid, photo.id).subscribe(() => {
      // removing all elememts that are not main photos
      this.currentMain = this.photos.filter(p => p.isMain === true)[0];
      this.currentMain.isMain = false;
      // setting latest photo as main
      photo.isMain = true;
      this._auth.changeMemberPhoto(photo.url);
      this._auth.currentUser.photoUrl = photo.url;
      localStorage.setItem('user', JSON.stringify(this._auth.currentUser));
    }, error => {
      this._alert.error(error);
    });
  }

  deletePhoto(id: number) {
    this._alert.confirm('Are you sure you want to delete this photo?', () => { // call back will execute which has the deletion and
                                                                              // then handling success
      this._user.deletePhoto(this._auth.decodedToken.nameid, id).subscribe(() => {
        this.photos.splice(this.photos.findIndex(p => p.id === id), 1);
        this._alert.success('Photo deleted');
      }, error => {
        this._alert.error('Failed to delete photo');
      });
    });
  }
}
