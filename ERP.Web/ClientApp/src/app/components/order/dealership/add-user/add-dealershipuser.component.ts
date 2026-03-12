import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { AuthenticationService } from '../../../../Auth/authentication.service';
import { UserService } from '../../../user-management/user.service';

@Component({
  selector: 'app-add-dealershipuser',
  templateUrl: './add-dealershipuser.component.html',
  styleUrl: './add-dealershipuser.component.css',
  standalone: false
})
export class AddDealershipUserComponent {
  userForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  selectedRolls : any;
  officeList: any;
  PasswordNew: boolean = false;
  currentUser: any;
  passwordRequirements = {
    hasLowerCase: false,
    hasUpperCase: false,
    hasNumber: false,
    hasMinimumLength: false,
    hasSpecialCharacter: false
  };

  constructor( private userService: UserService,private dialog: MatDialog,private notificationsService: NotificationsService,private formBuilder: FormBuilder,private authService : AuthenticationService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }){}

  ngOnInit(): void {
    this.userForm = this.formBuilder.group({
      id: [0,Validators.required],
      firstName: ['',Validators.required],
      lastName: [''],
      email: [''],
      userName: [''],
      userId: [''],
      phoneNumber : [''],
      isEmployee : false,
      IsAvailableForMobile : true,
      departmentId: ['12'],
      password: [
        '',
        [
          Validators.pattern('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&].{8,}')
         ]
      ],
      roleId: [['039B4FEC-1C2B-45D5-B438-DA0A4A8C2340']],
      dealershipId: [0,Validators.required],
      isActive: [true,Validators.required]
    });
     this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.LoadData(this.data.element);

    this.userForm.get('password')?.valueChanges.subscribe(value => {
      this.checkPasswordRequirements(value);
    });
  }

  checkPasswordRequirements(password: string) {
    this.passwordRequirements.hasLowerCase = /[a-z]/.test(password);
    this.passwordRequirements.hasUpperCase = /[A-Z]/.test(password);
    this.passwordRequirements.hasNumber = /[0-9]/.test(password);
    this.passwordRequirements.hasMinimumLength = password.length >= 8;
    this.passwordRequirements.hasSpecialCharacter = /[!@#$%^&*(),.?":{}|<>]/.test(password);
  }

  PasswordVisibilityNew(): void {
    this.PasswordNew = !this.PasswordNew;
  }

  LoadData(element: any) {
    if (element.user != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element.user, this.userForm);
      this.userForm.get('userId')?.patchValue(element.user?.id);
    }
    else{
      this.userForm.get('phoneNumber')?.patchValue(element.phoneNo);
      this.userForm.get('dealershipId')?.patchValue(element.id);
      // this.userForm.get('companyId')?.patchValue(this.currentUser.companyId);
      var namearray = element.name.trim().split(' ');
      if(namearray.length > 1){
      this.userForm.get('firstName')?.patchValue(namearray[0]);
      this.userForm.get('lastName')?.patchValue(namearray[1]);
      }
      else{
       this.userForm.get('firstName')?.patchValue(namearray[0]);
      this.userForm.get('lastName')?.patchValue(namearray[0]);
      }

    }
  }

  async SaveData() {
    if (this.userForm.invalid) {
      this.constantService.markFormGroupTouched(this.userForm);
      return;
    }

    this.isLoading = true;
    let _userForm: any = {};
    _userForm = Object.assign(_userForm, this.userForm.value);

    if (_userForm.id === 0) {
      (await this.authService.register(_userForm)).subscribe({
        next: (data : any) => {
          this.handleResponse(data);
        },
        error: (error) => {
          this.notificationsService.showNotification(error, 'snack-bar-danger');
          this.isLoading = false;
        }
      });
    } else {
        (await this.userService.changeUserPassword(_userForm)).subscribe({
      next: (data : any) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification('Password Changed Successfully', 'snack-bar-success');
          this.dialog.closeAll();
        }
        else
          this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error : any) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        this.isLoading = false;
      }
    });
    }
  }

  handleResponse(data: any) {
    if (data.Status == 200) {
      this.notificationsService.showNotification(data.Data, 'snack-bar-success');
      this.dialog.closeAll();
    } else {
      this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
    }
    this.isLoading = false;
  }
}
