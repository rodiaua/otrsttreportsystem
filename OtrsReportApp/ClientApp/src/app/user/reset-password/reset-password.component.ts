import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss'],
  providers: [MessageService]
})
export class ResetPasswordComponent implements OnInit {

  
  formModel: FormGroup;
  constructor(public ref: DynamicDialogRef, public config: DynamicDialogConfig, private formBuilder: FormBuilder,
     private uServise: UserService, private messageService: MessageService) {
    this.formModel = this.formBuilder.group( {
      passwords: this.formBuilder.group({
        newPassword: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['',[Validators.required]]
    }, {validator: this.isValidPassword, })
    });
   }

  
  ngOnInit(): void {
    
  }

  isValidPassword(formGroup: FormGroup){
    let confirmPasswordCtrl = formGroup.get("confirmPassword");
    let password = formGroup.get("newPassword");
    //password Mismatch
    if(confirmPasswordCtrl.errors == null || 'passwordMismatch' in confirmPasswordCtrl.errors){
        if(password.value != confirmPasswordCtrl.value)
        confirmPasswordCtrl.setErrors({passwordMismatch: true});
        else 
        confirmPasswordCtrl.setErrors(null)
    }
    if(password.errors == null || 'lowerCase' in password.errors){
        if(!/[a-z]|[а-я]/.test(password?.value)) password.setErrors({lowerCase: true});
        else password.setErrors(null);
    }
}


  onSubmit(){
    if(this.formModel.valid){
      this.uServise.resetPassword(this.formModel.value.passwords?.newPassword, this.config.data.id).
      subscribe((res: any) => {
        if(res.succeeded){
          this.showSuccess('Password', 'The password reset successfully!');
          setTimeout(() => this.ref.close(), 2000);
        }else {
          res.errors.forEach(element => {
            console.log(res);
            switch (element.code) {
              default:
                this.showError("Oops!",element.description)
                break;
            }
            
          });
        }
      },
      err => {
        console.log(err);
      }
    );

    }
  }

  showSuccess(sum: string,message: string) {
    this.messageService.add({severity:'success', summary: sum, detail: message});
  }

  showError(sum: string,message: string) {
    this.messageService.add({severity:'error', summary: sum, detail: message});
}

}
