import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { UserService } from '../../user.service';
import { CompanyService } from '../../../company/company.service';
import { createMask } from '@ngneat/input-mask';
import { DepartmentService } from '../../../department/department.service';
import { StoreService } from '../../../store/store.service';

@Component({
    selector: 'app-add-user',
    templateUrl: './add-user.component.html',
    styleUrl: './add-user.component.css',
    standalone: false
})

export class AddUserComponent {
  userForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  rolesList: any;
  selectedRolls: any;
  companyList: any;
  departmentList: any;
  storeList : any;
  PasswordNew: boolean = false;
  emailInputMask = createMask('*[*{0,50}]@*[*{0,50}].*[*{0,5}]');
  isAdmin: boolean = false;

  passwordRequirements = {
    hasLowerCase: false,
    hasUpperCase: false,
    hasNumber: false,
    hasMinimumLength: false,
    hasSpecialCharacter: false
  };

  constructor( private storeService: StoreService, private departmentService: DepartmentService, private companyService: CompanyService, private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private userService: UserService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.isAdmin = false; // Ensure a proper value
    this.userForm = this.formBuilder.group({
      id: [0, Validators.required],
      firstName: ['', Validators.required],
      lastName: [''],
      email: [''],
      userName: [''],
      password: [
        '',
        [
          Validators.pattern('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&].{8,}')
        ]
      ],
      roleId: [''],
      companyId: [''],
      departmentId: [''],
      storeId: [''],
      title: [''],
      phoneNumber: [''],
      isActive: [true, Validators.required]
    });
    this.LoadData(this.data.element);
    this.getRolesList();
    this.getCompanyList();

    this.userForm.get('password')?.valueChanges.subscribe(value => {
      this.checkPasswordRequirements(value);
    });
  }

  checkPasswordRequirements(password: string) {
    this.passwordRequirements.hasLowerCase = /[a-z]/.test(password);
    this.passwordRequirements.hasUpperCase = /[A-Z]/.test(password);
    this.passwordRequirements.hasNumber = /[0-9]/.test(password);
    this.passwordRequirements.hasMinimumLength = password.length >= 9;
    this.passwordRequirements.hasSpecialCharacter = /[!@#$%^&*(),.?":{}|<>]/.test(password);
  }

  PasswordVisibilityNew(): void {
    this.PasswordNew = !this.PasswordNew;
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
  
      this.constantService.LoadData(element, this.userForm);
      this.getDepartmentList();
      this.getStoreList();
    }
  }

  getRolesList(): void {
    this.userService.getAllRoles().subscribe(data => {
      this.rolesList = data;
      if (this.data.element != null) {
        // Map IDs to their respective names
        const selectedRoleNames = this.rolesList
          .filter((role: { id: any; }) => this.data.element.roleId[0].includes(role.id)) // Find roles that match selected IDs
          .map((role: { name: any; }) => role.name); // Extract the role names

        if (selectedRoleNames[0] == 'Admin')
          this.isAdmin = true;
        console.log('Selected Role Names:', selectedRoleNames);
      }
    });
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
      (await this.userService.register(_userForm)).subscribe({
        next: (data) => {
          this.handleResponse(data);
        },
        error: (error) => {
          this.notificationsService.showNotification(error, 'snack-bar-danger');
          this.isLoading = false;
        }
      });
    } else {
      this.userService.updateUser(_userForm).subscribe({
        next: (data) => {
          this.handleResponse(data);
        },
        error: (error) => {
          this.notificationsService.showNotification(error, 'snack-bar-danger');
          this.isLoading = false;
        }
      });
    }
  }

  handleResponse(data: any) {
    if (data.Status == 200) {
      this.notificationsService.showNotification(data.Message, 'snack-bar-success');
      this.dialog.closeAll();
    } else {
      this.notificationsService.showNotification(data.Message, 'snack-bar-danger');
    }
    this.isLoading = false;
  }
  
  getCompanyList(): void {
    let _companyForm: any = {};
    this.companyService.getAllCompanys(_companyForm).subscribe(data => {
      this.companyList = data.item1;
    });
  }

  getDepartmentList(): void {
    var companyId = this.userForm.get('companyId')?.value;
    this.departmentService.getDepartmentByCompany(companyId).subscribe(data => {
      this.departmentList = data;
    });
  }

  getStoreList(): void {
    var companyId = this.userForm.get('companyId')?.value;
    this.storeService.getStoreByCompany(companyId,false).subscribe(data => {
      this.storeList = data;
    });
  }

  checkRole(event: any) {
    // Assuming event contains the array of selected role IDs
    const selectedIds = event.value; // This gives the selected role IDs (array)
  
    // Map IDs to their respective names
    const selectedRoleNames = this.rolesList
      .filter((role: { id: any; }) => selectedIds.includes(role.id)) // Find roles that match selected IDs
      .map((role: { name: any; }) => role.name); // Extract the role names
  
      if(selectedRoleNames[0] == 'Admin')
        this.isAdmin = true;
    console.log('Selected Role Names:', selectedRoleNames);
    // You can assign `selectedRoleNames` to a variable or use it as needed
  }
}
