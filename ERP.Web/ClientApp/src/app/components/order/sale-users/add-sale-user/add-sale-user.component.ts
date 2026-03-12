import { Component, Inject } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { CompanyService } from '../../../company/company.service';
import { createMask } from '@ngneat/input-mask';
import { DepartmentService } from '../../../department/department.service';
import { StoreService } from '../../../store/store.service';
import { DealershipService } from '../../../order/dealership/dealership.service';
import { AuthenticationService } from '../../../../Auth/authentication.service';
import { UserService } from '../../../user-management/user.service';
import { EmployeeDesignationService } from '../../../hr/employee-designation/employee-designation.service';

@Component({
  selector: 'app-add-sale-user',
  templateUrl: './add-sale-user.component.html',
  styleUrl: './add-sale-user.component.css',
  standalone: false
})

export class AddSaleUserComponent {
  userForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  rolesList: any;
  selectedRolls: any;
  companyList: any;
  departmentList: any;
  storeList: any;
  PasswordNew: boolean = false;
  emailInputMask = createMask('*[*{0,50}]@*[*{0,50}].*[*{0,5}]');
  employeeDesignationList: any;
  passwordRequirements = {
    hasLowerCase: false,
    hasUpperCase: false,
    hasNumber: false,
    hasMinimumLength: false,
    hasSpecialCharacter: false
  };

  // Fields for KC Users (SALE)
  emailMask = createMask({ alias: 'email' });
  phoneNoInputMask = createMask('0399-9999999');
  cnicInputMask = createMask('99999-9999999-9');
  isDealerRole: Boolean = false;;
  imageSrc: any;
  dealershipList: any;
  isSectionDisabled = false;
  days: string[] = ['Friday', 'Saturday', 'Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday'];

  currentUser: any;
  disableCompanyDropdown = true;

  constructor(private storeService: StoreService, private departmentService: DepartmentService, private companyService: CompanyService, private dialog: MatDialog,
    private employeeDesignationService: EmployeeDesignationService, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private userService: UserService, private constantService: ConstantService,
    private dealershipService: DealershipService, private authenticationService: AuthenticationService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  async ngOnInit(): Promise<void> {
    this.currentUser = this.authenticationService.currentUserValue;

    if (this.currentUser.department === null) {
      this.disableCompanyDropdown = false;
    }
    else {
      this.disableCompanyDropdown = true;
    }

    this.userForm = this.formBuilder.group({
      id: [0, Validators.required],
      firstName: ['', Validators.required],
      lastName: [''],
      email: [''],
      userName: [''],
      password: [
        'Kc@123456',
        [
          Validators.pattern('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&].{8,}')
        ]
      ],
      roleId: [''],
      companyId: new FormControl({ value: '', disabled: this.disableCompanyDropdown }),
      departmentId: new FormControl({ value: '', disabled: true }),
      department: [''],
      storeId: [''],
      title: [''],
      phoneNumber: [''],
      isActive: [true, Validators.required],
      joinDate: ['', Validators.required],
      dateOfConfirmation: ['', Validators.required],
      // Fields for KC Users (SALE)
      dateOfBirth: ['', Validators.required],
      emergencyPhoneNo: ['', Validators.required],
      bloodGroup: ['', Validators.required],
      cnic: ['', Validators.required],
      shiftTimeStart: [null, Validators.required],
      shiftTimeEnd: [null, Validators.required],
      address: ['', Validators.required],
      imageName: [''],
      fileSource: [''],
      extension: [''],
      dealershipId: [null],
      deviceId: [''],
      isMobileDeviceRegister: [],
      isAvailableForMobile: [true],
      isAvailableForWeb: [true],
      isDistCompForAtten: [true],
      weeklyOff: ['', Validators.required],
      employeeDesignationId: ['', Validators.required],
    });

    await this.getRolesList();
    this.getCompanyList();
    this.getEmployeeDesignationList();
    this.userForm.get('password')?.valueChanges.subscribe(value => {
      this.checkPasswordRequirements(value);
    });

    this.getAllDealers();

    if (this.disableCompanyDropdown === true) {
      this.userForm.get('companyId')?.patchValue(this.currentUser.department?.companyId);
      this.getDepartmentList();
    }

    this.LoadData(this.data.element);
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

      this.userForm.get("weeklyOff")?.patchValue(this.data.element.weeklyOff?.split(","));
      let convertDate = this.constantService.formatDate(this.data.element.dateOfBirth);
      this.userForm.get("dateOfBirth")?.patchValue(convertDate);


      this.imageSrc = this.data.element.attachments[0]?.imageName;
      this.userForm.get("fileSource")?.patchValue(this.data.element.attachments[0]?.imageName);

      const selectedRoleNames = this.rolesList
        .filter((role: { id: any; }) => element.roleId.includes(role.id)) // Find roles that match selected IDs
        .map((role: { name: any; }) => role.name); // Extract the role names

      if (selectedRoleNames.includes('Distributor')) {
        this.isDealerRole = true;
      }
      else {
        this.isDealerRole = false;
      }

      this.getDepartmentList();
      this.getStoreList();

      setTimeout(() => {
        this.departmentChange();
      }, 500);
    }
  }

  async getRolesList(): Promise<void> {
    try {
      const data: any = await this.userService.getAllRolesByDepartment(12).toPromise(); // Convert Observable to Promise using toPromise()
      this.rolesList = data;

      if (this.data.element != null) {
        // Map IDs to their respective names
        const selectedRoleNames = this.rolesList
          .filter((role: { id: any }) => this.data.element.roleId[0].includes(role.id)) // Find roles that match selected IDs
          .map((role: { name: any }) => role.name); // Extract the role names

        console.log('Selected Role Names:', selectedRoleNames);
      }
    } catch (error) {
      console.error('Error fetching roles:', error);
    }
  }

  async SaveData() {
    if (this.userForm.invalid) {
      this.checkInvalidControls(this.userForm);
      this.constantService.markFormGroupTouched(this.userForm);
      return;
    }

    this.isLoading = true;
    let _userForm: any = {};

    _userForm = Object.assign(_userForm, this.userForm.value);
    _userForm.weeklyOff = _userForm.weeklyOff.join(",");

    let joinDate = this.constantService.formatDate(this.userForm.get('joinDate')?.value);
    _userForm['joinDate'] = joinDate;

    let dateOfConfirmation = this.constantService.formatDate(this.userForm.get('dateOfConfirmation')?.value);
    _userForm['dateOfConfirmation'] = dateOfConfirmation;

    let dateOfBirth = this.constantService.formatDate(this.userForm.get('dateOfBirth')?.value);
    _userForm['dateOfBirth'] = dateOfBirth;

    if (_userForm.id === 0) {
      _userForm.departmentId = 12;
      _userForm.employeeWorkSiteTypeId = 2;
      _userForm.isEmployee = 1;
      (await this.userService.register(_userForm)).subscribe({
        next: (data: any) => {
          this.handleResponse(data);
        },
        error: (error: string) => {
          this.notificationsService.showNotification(error, 'snack-bar-danger');
          this.isLoading = false;
        }
      });
    } else {
      _userForm.departmentId = 12;
      _userForm.employeeWorkSiteTypeId = 2;
      _userForm.isEmployee = 1;
      this.userService.updateUser(_userForm).subscribe({
        next: (data: any) => {
          this.handleResponse(data);
        },
        error: (error: string) => {
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
      this.userForm.get('departmentId')?.patchValue(12);
      this.departmentChange();
    });
  }

  getStoreList(): void {
    var companyId = this.userForm.get('companyId')?.value;
    this.storeService.getStoreByCompany(companyId, false).subscribe(data => {
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

    console.log('Selected Role Names:', selectedRoleNames);

    if (selectedRoleNames.includes('Distributor')) {
      this.isDealerRole = true;
    }
    else {
      this.isDealerRole = false;
    }
  }

  onFileChange(event: any) {

    const reader = new FileReader();
    if (event.target.files && event.target.files.length) {
      const [file] = event.target.files;
      reader.readAsDataURL(file);

      reader.onload = () => {
        this.imageSrc = reader.result as string;

        // this.userForm.patchValue({
        //   fileSource: reader.result,
        //   imageName: file.name
        // });

        this.userForm.get('imageName')?.patchValue(file.name);
        this.userForm.get('fileSource')?.patchValue(reader.result);
        this.userForm.get('extension')?.patchValue(file.name.split('.').pop().toLowerCase());
      };
    }
  }

  onFileSourceRemove(event: any) {
    this.userForm
      .get('imageName')?.patchValue('');

    this.userForm
      .get('fileSource')?.patchValue('');

    this.imageSrc = '';
  }

  shiftTimeCheck() {

    const startTime = this.userForm.get('shiftTimeStart')?.value;
    const endTime = this.userForm.get('shiftTimeEnd')?.value;

    if (startTime && endTime) {
      const start = this.convertToMinutes(startTime);
      const end = this.convertToMinutes(endTime);

      if (start >= end) {
        // Set end time to null and mark it as an error
        this.userForm.get('shiftTimeEnd')?.setValue(null);
        this.userForm.get('shiftTimeEnd')?.setErrors({ invalidTime: true });
        this.notificationsService.showNotification('Shift Time End can not be greater than Shift Time Start!', 'snack-bar-danger');
      } else {
        this.userForm.get('shiftTimeEnd')?.setErrors(null);
      }
    }
  }

  private convertToMinutes(time: string): number {
    const [hours, minutes] = time.split(':')?.map(Number);
    return hours * 60 + minutes;
  }

  // Custom validator to check if the date is not in the future
  dobCheck() {
    const selectedDate = new Date(this.userForm.get('dateOfBirth')?.value);
    const today = new Date();
    today.setHours(0, 0, 0, 0); // Set time to midnight for comparison

    // Check if the selected date is in the future
    if (selectedDate > today) {
      this.userForm.get('dateOfBirth')?.setValue(null);
      this.notificationsService.showNotification('Date of Birth can not be future date!', 'snack-bar-danger');
    }
  }

  async getAllDealers() {
    let _dealershipListFilerForm: any = {
      'dealershipTypeId': 1
    };
    (await (this.dealershipService.getAllDealership(_dealershipListFilerForm))).subscribe(
      {
        next: (data: { item1: any; }) => {
          this.dealershipList = data.item1;
          this.isLoading = false;
        },
        error: (error: any) => {
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  checkInvalidControls(formGroup: FormGroup) {
    // Loop through each control in the FormGroup
    Object.keys(formGroup.controls).forEach(controlName => {
      const control = formGroup.get(controlName);

      // Check if the control is invalid
      if (control && control.invalid) {
        console.log(`Control '${controlName}' is invalid.`);
        // You can further log the specific errors for each control
        console.log(control.errors);
      }
    });

    // If there are FormArrays, check their controls as well
    if (formGroup instanceof FormArray) {
      formGroup.controls.forEach((formControl, index) => {
        if (formControl.invalid) {
          console.log(`FormArray control at index ${index} is invalid.`);
          console.log(formControl.errors);
        }
      });
    }
  }

  departmentChange() {
    let department = this.departmentList.filter((x: any) => x.id === this.userForm.get('departmentId')?.value)[0];
    this.userForm.get('department')?.patchValue(department.name);
  }


  getEmployeeDesignationList(): void {
    let _filterForm = {};
    this.employeeDesignationService.getAllEmployeeDesignations(_filterForm).subscribe(data => {
      this.employeeDesignationList = data.item1;
    });
  }


}
