import { Component, OnInit } from '@angular/core';
import { FormGroup, NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { UserService } from '../services/user.service';
import { TwoFactorAuthenticationComponent } from './two-factor-authentication/two-factor-authentication.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  providers: [MessageService, DialogService]
})
export class LoginComponent implements OnInit {

  formModel: any;
  ref: DynamicDialogRef;

  constructor(private service: UserService, private router: Router, private messageService: MessageService, public dialogService: DialogService) { 
    this.formModel = {
      username: '',
      password: '',
    }
  }

  ngOnInit(): void {
    if(localStorage.getItem('token')){
      this.router.navigateByUrl('/reportTable');
    }
  }

  onSubmit(form: NgForm){
    this.service.login(form.value).subscribe(
      (res:any) => {
        localStorage.setItem("otpToken", res.otpToken);
        this.showTwoFactorDialog();
      },
      err=>{
        if(err.status == 400){
          console.log(err);
          this.showError("Authentication failed", "Incorrect username or password");
        }else{
          console.log(err);
        }
      }
    )
  }

  async showTwoFactorDialog(){
    this.ref = this.dialogService.open(TwoFactorAuthenticationComponent, {
      header: 'Two Factor Authentication',
      width: '700px',
      contentStyle: {"max-height": "500px", "overflow": "auto"},
      baseZIndex: 10000
  });
  }

  showError(sum: string,message: string) {
    this.messageService.add({severity:'error', summary: sum, detail: message});
  }

}
