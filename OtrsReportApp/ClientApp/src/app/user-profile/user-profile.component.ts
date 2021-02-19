import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';

import { UserService } from '../services/user.service';
import { ChangePasswordComponent } from './change-password/change-password.component';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss'],
  providers: [MessageService, DialogService]
})
export class UserProfileComponent implements OnInit, OnDestroy {

  values: any[];
  userProfile: any;
  editing: boolean;
  formModel: FormGroup;
  ref: DynamicDialogRef;
  error: string;

  @ViewChild('form') profileForm: NgForm;

  constructor(private service: UserService, private messageService: MessageService, private formBuilder: FormBuilder,
    public dialogService: DialogService) {
    this.editing = false;
  }

  async ngOnInit() {
    this.userProfile = await this.service.getUserProfile();
    this.formModel = this.formBuilder.group( {
      userName: [this.userProfile.userName],
      email: [this.userProfile.email, [Validators.required, Validators.email]],
      firstName: [this.userProfile.firstName],
      lastName: [this.userProfile.lastName]
    });
    this.values = [this.userProfile];
    this.onChanges();
  }

  onChanges(): void {
    this.profileForm.valueChanges.subscribe(val => {
      if(this.userProfile.email != this.formModel.value.email || 
        this.userProfile.userName != this.formModel.value.userName || 
        this.userProfile.firstName != this.formModel.value.firstName || 
        this.userProfile.lastName != this.formModel.value.lastName){
        this.editing = true;
      }else this.editing = false;
    });
  }

  onFocus() {
    this.editing = true;
  }

  async onSubmit(form: NgForm) {
    if(form.valid){
      this.service.updateUserProfile(this.formModel.value).subscribe(
        (res: any) => {
          if (res.succeeded) {
            this.showSuccess('Successful profile update', 'The user profile is updated successfully!');
              this.userProfile = {
                userName: this.formModel.value.userName,
                email: this.formModel.value.email,
                firstName: this.formModel.value.firstName,
                lastName: this.formModel.value.lastName
              };
              this.editing = false;
            this.error = '';
          } else {
            res.errors.forEach(element => {
              console.log(res);
              switch (element.code) {
                case 'DuplicateUserName':
                  this.showError("Oops!", "The username is already taken!")
                  break;
                default:
                  this.showError("Oops!", element.description)
                  break;
              }
  
            });
          }
        },
        err => {
          console.log(err);
        }
      );
    }else{
      if(form.form.get('email')?.touched && form.form.get('email')?.errors?.required){
        this.showError("Profile update fail", "The email is required");
        this.formModel.value.email = this.userProfile.email;
      } else if(form.form.get('email')?.touched && form.form.get('email')?.errors?.email){
        this.showError("Profile update fail", "The email is incorrect");
        this.formModel.value.email = this.userProfile.email;
      }
      if(form.form.get('userName')?.touched && form.form.get('userName')?.errors?.required){
        this.showError("Profile update fail", "The username is required");
        this.formModel.value.userName = this.userProfile.userName;
      }
      console.log(this.formModel.get('email').errors);
      this.editing = false;
    }
  }

  showSuccess(sum: string, message: string) {
    this.messageService.add({ severity: 'success', summary: sum, detail: message });
  }

  showError(sum: string, message: string) {
    this.messageService.add({ severity: 'error', summary: sum, detail: message });
  }

  onReset(){
    this.formModel = this.formBuilder.group( {
      userName: [this.userProfile.userName],
      email: [this.userProfile.email, [Validators.required, Validators.email]],
      firstName: [this.userProfile.firstName],
      lastName: [this.userProfile.lastName]
    });
    this.editing = false;
  }


  showChangePasswordDialog(){
    this.ref = this.dialogService.open(ChangePasswordComponent, {
      header: 'Change Password',
      width: '50%',
      contentStyle: {"max-height": "500px", "overflow": "auto"},
      baseZIndex: 10000
  });
  }

  ngOnDestroy() {
    if (this.ref) {
        this.ref.close();
    }
  }
}
