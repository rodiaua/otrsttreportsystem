import { Injectable, OnInit } from "@angular/core";
import {  FormBuilder, FormGroup, Validators } from "@angular/forms";
import {  HttpClient } from "@angular/common/http";
import {UserTableModel} from '../models/dto/user-table-model';
import { Password } from 'primeng/password';

@Injectable({ providedIn: 'root' })
export class UserService implements OnInit{

    readonly url = "/api/appUser";
    readonly urlUserProfile = "/api/userProfile";

    /**
     *
     */
    formModel: FormGroup;

    constructor(private formBuilder: FormBuilder, private http: HttpClient) {
        this.formModel = this.formBuilder.group({
            email: ['',[Validators.required, Validators.email]],
            firstName: [''],
            lastName: [''],
            passwords: this.formBuilder.group({
                password: ['', [Validators.required, Validators.minLength(6)]],
                confirmPassword: ['',[Validators.required]]
            }, {validator: this.isValidPassword, }),
            isAdmin: [false]
        });
    }
    ngOnInit(): void {
    }


    isValidPassword(fb: FormGroup){
        let confirmPasswordCtrl = fb.get("confirmPassword");
        let password = fb.get("password");
        //password Mismatch
        if(confirmPasswordCtrl.errors == null || 'passwordMismatch' in confirmPasswordCtrl.errors){
            if(password.value != confirmPasswordCtrl.value)
            confirmPasswordCtrl.setErrors({passwordMismatch: true});
            else 
            confirmPasswordCtrl.setErrors(null)
        }
        if(password.errors == null || 'lowerCase' in password.errors){
            if(!/[a-z]|[а-я]/.test(password.value)) password.setErrors({lowerCase: true});
            else password.setErrors(null);
        }
    }

    register(){
        var body = {
            email: this.formModel.value.email,
            firstName: this.formModel.value.firstName,
            lastName: this.formModel.value.lastName,
            password: this.formModel.value.passwords?.password,
            role: this.formModel.value.isAdmin == true ? "Admin" : "User"
        }
        return this.http.post(this.url+'/register', body);
    }

    login(userLoginModel){
        return this.http.post(this.url+'/login', userLoginModel);
    }

    getUserProfile(){
        return this.http.get(this.urlUserProfile+'/getUserProfile').toPromise();
    }

    roleMatch(allowedRoles){
        var match = false;
        var payLoad = JSON.parse(window.atob(localStorage.getItem("token").split(".")[1]));
        var userRole = payLoad.role;
        allowedRoles.forEach(role => {
            if(userRole == role){
                match = true;
                return;
            }
        });
        return match;
    }

    getUserRole(){
        if(localStorage.getItem("token")){
            var payLoad = JSON.parse(window.atob(localStorage.getItem("token").split(".")[1]));
            return payLoad.role;
        } 
    }

    userConfirmed(){
        if(localStorage.getItem("token")){
            var payLoad = JSON.parse(window.atob(localStorage.getItem("token").split(".")[1]));
            return payLoad.role == undefined? false : true;
        } 
    }

    updateUserProfile(profileModel){
        return this.http.post(this.urlUserProfile+'/updateProfile', profileModel);
    }

    changePassword(currentPassword: string, newPassword){
        return this.http.post(this.urlUserProfile+'/changePassword', {currentPassword, newPassword});
    }

    getUsers(){
        return this.http.get<UserTableModel[]>(this.url+'/getUsers').toPromise();
    }

    updateUser(userModel: UserTableModel){
        return this.http.post(this.url+'/updateUser', userModel);
    }


    resetPassword(password: string, id: string){
        return this.http.post(this.url+'/resetPassword', {id,password});
    }

    deleteUser(id: string){
        return this.http.get(this.url+'/deleteUser/'+id).toPromise();
    }

    confirmOtp(otp: string){
        return this.http.get(this.url+'/confirmOtp/'+otp);
    }

    removeOtps(){
        return this.http.get(this.url+'/removeOtps/').toPromise();
    }
    

}