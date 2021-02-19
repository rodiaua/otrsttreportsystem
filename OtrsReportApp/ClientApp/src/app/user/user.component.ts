import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';
import { UserTableModel } from '../models/dto/user-table-model';
import { UserService } from '../services/user.service';
import { ResetPasswordComponent } from './reset-password/reset-password.component';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss'],
  providers: [MessageService, DialogService]
})
export class UserComponent implements OnInit, OnDestroy {

  users: UserTableModel[];
  userForm: FormGroup;
  editingRows: any[];
  ref: DynamicDialogRef;

  @ViewChild('dt') table:Table

  constructor(private userService: UserService, private formBuilder: FormBuilder, private messageService: MessageService,public dialogService: DialogService) {
    this.userService.getUsers().then(res => {
      this.users = res;
      this.createUserForm();
    });
    this.editingRows = [];

  }

  async ngOnInit() {
 
  }

  private createUserForm() {
    this.userForm = this.formBuilder.group({
      tableRowArray: this.formBuilder.array(this.createTableRowArray())
    });
  }

  get tableRowArray(): FormArray{
    return this.userForm.get('tableRowArray') as FormArray;
  }

  private createTableRowArray(): FormGroup[] {
    if (this.users.length > 0) {
      let tableRows = [];
      this.users.forEach(user => {
        tableRows.push(this.formBuilder.group({
          rowId: [this.users.indexOf(user)],
          userId: [user.id],
          userName:new FormControl (user.userName, Validators.required),
          email: new FormControl(user.email, [Validators.required, Validators.email]),
          firstName:new FormControl(user.firstName),
          lastName:new FormControl(user.lastName),
          role: new FormControl(user.role),
        }))
      });
      return tableRows;
    }
  }


  

  onRowEditInit(rowData: FormGroup) {
    this.editingRows.push(rowData.value);
  }

  onRowEditSave(rowData: FormGroup) {
    if(rowData.valid){
      let user: UserTableModel = {
        id: rowData.value.userId,
        email: rowData.value.email,
        userName: rowData.value.userName,
        firstName: rowData.value.firstName,
        lastName: rowData.value.lastName,
        role: rowData.value.role,
      };
      this.userService.updateUser(user).subscribe((res: any) => {
        if (res.updateUserProfileResult.succeeded) {
          if(res?.updateUserRoleResult?.error?.length > 0 || res?.updateUserProfileResult?.error?.length > 0)console.log(res);
          this.showSuccess('Successful profile update', 'The user profile is updated successfully!');
        } else {
          if(res?.updateUserRoleResult?.error?.length > 0 || res?.updateUserProfileResult?.error?.length > 0)console.log(res);
          res.updateUserProfileResult.errors.forEach(element => {
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
      })
    }
  }

  onRowEditCancel(rowData: FormGroup, rowId: number) {
    let id = rowData.value.userId;
    let row = this.editingRows.find(row => row.userId == id);
    this.tableRowArray.controls[rowData.value.rowId].setValue(row);
  }

  showResetPasswordDialog(rowData){
    this.ref = this.dialogService.open(ResetPasswordComponent, {
      data:{
        id: rowData.value.userId
      },
      header: 'Reset Password',
      width: '50%',
      contentStyle: {"max-height": "500px", "overflow": "auto"},
      baseZIndex: 10000
  });
  }

  showSuccess(sum: string, message: string) {
    this.messageService.add({ severity: 'success', summary: sum, detail: message });
  }

  showError(sum: string, message: string) {
    this.messageService.add({ severity: 'error', summary: sum, detail: message });
  }

  ngOnDestroy() {
    if (this.ref) {
        this.ref.close();
    }
  }

  async onUserDelete(rowData){
    await this.userService.deleteUser(rowData.value.userId).then((res:any) => {
      if (res.succeeded) {
        this.showSuccess('User remove', 'The user is removed!');
        this.userService.getUsers().then(res => {
          this.users = res;
          this.createUserForm();
        });
      } else {
        res.errors.forEach(element => {
          console.log(res);
          switch (element.code) {
            default:
              this.showError("Oops!", element.description)
              break;
          }

        });
      }
    },
    err => {
      console.log(err);
    });
    
  }
}
