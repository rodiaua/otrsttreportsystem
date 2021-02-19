import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Observable, timer } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'two-factor-authentication',
  templateUrl: './two-factor-authentication.component.html',
  styleUrls: ['./two-factor-authentication.component.scss'],
  providers: [MessageService, DialogService]
})
export class TwoFactorAuthenticationComponent {

  formModel: FormGroup
  otpFailed: number;
  counter$: Observable<number>;
  count: number;

  constructor(private userService: UserService,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private formBuilder: FormBuilder,
    private messageService: MessageService,
    private router: Router) {
    this.ref.onDestroy.subscribe(() => {
      if(localStorage.getItem("otpToken")) this.userService.removeOtps().then(() => localStorage.removeItem("otpToken"));
    })
    this.formModel = this.formBuilder.group({
      otpCode: ['', Validators.required]
    });
    this.count = 160;
    this.otpFailed = 0;
    this.counter$ = timer(0, 1000).pipe(
      take(this.count),
      map(() => {
        if (this.count == 1) {
          this.showError("Confirmation fail", "Session expired");
          this.delay(1500).then(() => this.ref.destroy());
          return 0;
        }
        return --this.count
      })
    );
  }

  confirmOtp() {
    this.userService.confirmOtp(this.formModel.value.otpCode).subscribe((result: any) => {
      if (!result.succeeded) {
        if (result.error != undefined) {
          this.showError("Confirmation fail", result.error);
          if (result.error == "Otp code is expired") {
            this.delay(1500).then(() => this.ref.destroy());
          }
          else if (result.error == "Otp code is wrong") {
            this.otpFailed++;
            if (this.otpFailed >= 3) {
              this.delay(1500).then(() => this.ref.destroy());
            }
          }
          else {
            this.showError("Confirmation fail", "");
            this.delay(1500).then(() => this.ref.destroy());
          }
        }
      } else if (result.succeeded) {
        this.userService.removeOtps().then(() => {
          localStorage.setItem("token", result.token);
          this.router.navigateByUrl('/reportTable');
          this.ref.destroy();
        });
      }
    });
  }
  delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  showSuccess(sum: string, message: string) {
    this.messageService.add({ severity: 'success', summary: sum, detail: message });
  }

  showError(sum: string, message: string) {
    this.messageService.add({ severity: 'error', summary: sum, detail: message });
  }


}
