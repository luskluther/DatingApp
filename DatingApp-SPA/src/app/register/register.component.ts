import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker/ngx-bootstrap-datepicker';
import { User } from '../_models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  @Output() cancelRegister = new EventEmitter();
  user: User;
  registerForm: FormGroup;
  bsConfig: Partial<BsDatepickerConfig>; // date picker config but we are only implementing partially not all

  constructor(private _authService: AuthService,
              private _alertify: AlertifyService,
              private _fb: FormBuilder,
              private _router: Router) { }

  ngOnInit() {
    this.bsConfig = {
      containerClass: 'theme-default'
    };
    this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this._fb.group({ // initizlizing some form fields using form builder for the form
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', Validators.required]
    }, { validator: this.passwordMatchValidator }); // this is the custom validation for confirm password matching
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value ? null : {'mismatch' : true};
  }

  // in this we are chaining first we are registering and then directly moving to login members page if there is no error anywhere.
  register() {
    if (this.registerForm.valid) {
      // this clones values from resigter form to empty object and then we assign the empty object to this.user
      this.user = Object.assign({}, this.registerForm.value);
      this._authService.register(this.user).subscribe(() => {
        this._alertify.success('Registration successful');
      }, error => {
        this._alertify.error(error);
        // this is the complete callback for the register
      }, () => {
        this._authService.login(this.user).subscribe(() => {
          this._router.navigate(['/members']);
        });
      });
    }
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

}
