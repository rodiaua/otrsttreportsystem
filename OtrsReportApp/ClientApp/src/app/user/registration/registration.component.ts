import { Component, OnInit } from '@angular/core';
import { MessageService } from 'primeng/api';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss'], 
  providers: [MessageService]
})
export class RegistrationComponent implements OnInit {
  
  error: string;

  constructor(public userService: UserService, private messageService: MessageService) { }

  ngOnInit(): void {
    this.userService.formModel.reset();
    this.error = '';
  }

  onSubmit(){
    this.userService.register().subscribe(
      (res: any) => {
        if(res.succeeded){
          this.showSuccess('Success registration', 'The user registered successfully!');
          this.userService.formModel.reset();
          this.error = '';
        }else {
          res.errors.forEach(element => {
            console.log(res);
            switch (element.code) {
              case 'DuplicateUserName':
                this.showError("Oops!","The user with the email already exists!")
                break;
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

  showSuccess(sum: string,message: string) {
    this.messageService.add({severity:'success', summary: sum, detail: message});
  }

  showError(sum: string,message: string) {
    this.messageService.add({severity:'error', summary: sum, detail: message});
}

}
