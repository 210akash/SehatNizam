import { Component, Inject } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { createMask } from '@ngneat/input-mask';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { CompanyService } from '../../../company/company.service';
import { DepartmentService } from '../../../department/department.service';
import { StoreService } from '../../../store/store.service';
import { UserService } from '../../../user-management/user.service';
import { EmployeeDesignationService } from '../../employee-designation/employee-designation.service';
import { EmployeeEducationService } from '../../employee-education/employee-education.service';
import { EmployeeGradeService } from '../../employee-grade/employee-grade.service';
import { EmployeeShiftService } from '../../employee-shift/employee-shift.service';
import { EmployeeTypeService } from '../../employee-type/employee-type.service';
import { EmployeeBankService } from '../../employee-bank/employee-bank.service';
import { EmployeeLeaveGroupService } from '../../employee-leave-group/employee-leave-group.service';
import { EmployeeDocumentTypeService } from '../../employee-document-type/employee-document-type.service';
import { CityService } from '../../city/city.service';
import { EmployeeOvertimeRateService } from '../../employee-overtimerate/employee-overtimerate.service';
import { ProjectService } from '../../../project/project.service';
import { MatSlideToggleChange } from '@angular/material/slide-toggle';
import { EmployeeWorkSiteTypeService } from '../../employee-worksitetype/employee-worksitetype.service';
import { MatSelectChange } from '@angular/material/select';

@Component({
  selector: 'app-add-employee',
  templateUrl: './add-employee.component.html',
  styleUrl: './add-employee.component.css',
  standalone: false
})

export class AddEmployeeComponent {
  employeeForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  rolesList: any;
  selectedProjects: any;
  selectedRolls: any;
  companyList: any;
  departmentList: any;
  storeList: any;
  projectList: any
  PasswordNew: boolean = false;
  emailInputMask = createMask('*[*{0,50}]@*[*{0,50}].*[*{0,5}]');
  isAdmin: boolean = false;
  department: any;
  passwordRequirements = {
    hasLowerCase: false,
    hasUpperCase: false,
    hasNumber: false,
    hasMinimumLength: false,
    hasSpecialCharacter: false
  };

  imageSrc: any;
  cnicInputMask = createMask('99999-9999999-9');
  phoneNoInputMask = createMask('0399-9999999');
  employeeDesignationList: any;
  employeeEducationList: any;
  employeeGradeList: any;
  employeeShiftList: any;
  employeeTypeList: any;
  employeeSiteTypeList: any;
  employeeBankList: any;
  employeeLeaveGroupList: any;
  employeeDocumentTypeList: any;
  selectedLeaveGroup: any[] = [];
  cityList: any;
  currentUser: any;
  roleList: string | undefined;
  dayList = [
    { key: 'monday', label: 'Monday', check: true },
    { key: 'tuesday', label: 'Tuesday', check: true },
    { key: 'wednesday', label: 'Wednesday', check: true },
    { key: 'thursday', label: 'Thursday', check: true },
    { key: 'friday', label: 'Friday', check: true },
    { key: 'saturday', label: 'Saturday', check: true },
    { key: 'sunday', label: 'Sunday', check: false }
  ];
  employeeOvertimeRateList: any;

  constructor(private projectService: ProjectService, private storeService: StoreService, private departmentService: DepartmentService,
    private companyService: CompanyService, private dialog: MatDialog, private notificationsService: NotificationsService,
    private formBuilder: FormBuilder, private employeeService: UserService, private constantService: ConstantService, private cityService: CityService,
    private employeeDesignationService: EmployeeDesignationService, private employeeEducationService: EmployeeEducationService,
    private employeeGradeService: EmployeeGradeService, private employeeShiftService: EmployeeShiftService, private employeeLeaveGroupService: EmployeeLeaveGroupService,
    private employeeTypeService: EmployeeTypeService, private employeeBankService: EmployeeBankService, private employeeDocumentTypeService: EmployeeDocumentTypeService,
    private employeeOvertimeRateService: EmployeeOvertimeRateService,
    private employeeWorkSiteTypeService: EmployeeWorkSiteTypeService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.isAdmin = false; // Ensure a proper value
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());
    this.employeeForm = this.formBuilder.group({
      isActive: [true, Validators.required],
      imageName: [''],
      fileSource: [''],
      extension: [''],
      id: [0, Validators.required],
      hrCode: ['', Validators.required],
      firstName: ['', Validators.required],
      middleName: [''],
      lastName: [''],
      title: [''],
      password: [
        'Hms@123567'
      ],
      roleId: [[
      '46684446-1139-4085-6401-08ddf519c24b'
    ], Validators.required],
      selectedRolls: [['']],
      companyId: [this.currentUser.department?.companyId],
      gender: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      maritalStatus: [''],
      cnic: [''],
      cnicIssuanceDate: [null],
      cnicExpiryDate: [null],
      religion: [''],
      fatherHusbandName: [''],
      motherName: [''],
      spouseName: [''],
      spouseCnic: [''],
      child1: [''],
      child2: [''],
      bloodGroup: [''],
      employeeTypeId: ['', Validators.required],
      employeeWorkSiteTypeId: ['', Validators.required],
      workLocation: [''],
      departmentId: ['', Validators.required],
      storeId: [''],
      projectIds: [null],
      employeeDesignationId: ['', Validators.required],
      subDepartment: [''],
      joinDate: ['', Validators.required],
      employeeGradeId: [''],
      employeeShiftId: ['', Validators.required],
      // businessUnit: [''],
      // workLevel: [''],
      // lineManagerName: [''],
      // lineManagerDesignation: [''],
      country: ['Pakistan'],
      cityId: [''],
      address: [''],
      permanentAddress: [''],
      phoneNumber: [''],
      email: ['', Validators.required],
      emergencyPersonName: [''],
      emergencyPhoneNo: [''],
      emergencyRelation: [''],
      employeeBankId: [''],
      // bankName: [''],
      // branchName: [''],
      accountHolderName: [''],
      // accountNumber: [''],
      // branchCode: [''],
      bankAccountIBAN: [''],
      bankAccountNo: [''],
      employeeLeaveGroupId: ['', Validators.required],
      employeeOvertimeRateId: [''],
      // otherLeaves: [''],
      // emergencyLeave: [''],
      // sickLeave: [''],
      // casualLeave: [''],
      employeeEducationId: [''],
      overTimeAmount: [''],
      dateOfConfirmation: ['', Validators.required],
      // serviceStatusDescription: [''],
      documents: this.formBuilder.array([]),
      days: this.formBuilder.group({
        employeeId: [''],
        monday: [false],
        tuesday: [false],
        wednesday: [false],
        thursday: [false],
        friday: [false],
        saturday: [false],
        sunday: [false]
      }),
      isEmployee: [true],
      isResigned: [false],
      resignDate: [null],
      lastCompany: [''],
      relevantExperience: [''],
      totalWorkExperience: [''],
      reference: [''],
      remarks: [''],
      isRosterShift: [false],
      isMobileDeviceRegister: [],
      isAvailableForMobile: [false],
      isAvailableForWeb: [false],
      isDistCompForAtten: [false],
    });

   this.employeeForm.get('isRosterShift')?.valueChanges.subscribe(value => {
  if (value) {
    this.employeeForm.get('days')?.reset({
      employeeId: '',
      monday: false,
      tuesday: false,
      wednesday: false,
      thursday: false,
      friday: false,
      saturday: false,
      sunday: false
    });
  }
});

    this.handleResignValidation();
    this.handleDepartmentValidation();
    this.LoadData(this.data.element);
    this.getRolesList();
    this.getCompanyList();
    this.getDepartmentList();
    this.getStoreList();
    this.getprojectList();
    this.getEmployeeDesignationList();
    this.getEmployeeEducationList();
    this.getEmployeeGradeList();
    this.getEmployeeShiftList();
    this.getEmployeeTypeList();
    this.getEmployeeBankList();
    this.getEmployeeLeaveGroupsList();
    this.getEmployeeDocumentsList();
    this.getCityList();
    this.getEmployeeOvertimeRateList();
    this.getEmployeeSiteTypeList();
    if (!this.isEditMode) {
      this.employeeForm.get('password')?.valueChanges.subscribe(value => {
        this.checkPasswordRequirements(value);
      });
    }

    // const employeeRole = this.rolesList.find((role: { name: string; }) => role.name === 'Employee');
    // if (employeeRole) {
    //   this.employeeForm.get('roleId')?.patchValue([employeeRole.id])
    // }
  }

  get documentsFormArray(): FormArray {
    return this.employeeForm.get('documents') as FormArray;
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

      this.constantService.LoadData(element, this.employeeForm);
      this.department = this.data.element.department?.name;
      this.imageSrc = this.data.element.attachments[0]?.imageName;
      this.employeeForm.get("fileSource")?.patchValue(this.data.element.attachments[0]?.imageName);
      this.employeeForm.get('days')?.patchValue(element.employeeWorkingDays[0]);

      this.getDepartmentList();
      this.getprojectList();
      this.getStoreList();
    }
    else {
      // Patch the form group with values from dayList
      const dayValues: any = {};
      this.dayList.forEach(day => {
        dayValues[day.key] = day.check;
      });

      // Apply the day values to the 'days' form group
      this.employeeForm.get('days')?.patchValue(dayValues);

      this.isEditMode = false;
      this.employeeForm.get('isEmployee')?.patchValue(true);
      this.employeeForm.get("password")?.setValidators([Validators.required]);
      this.employeeForm.get("password")?.updateValueAndValidity();
    }
  }

  getRolesList(): void {
    this.employeeService.getAllRoles().subscribe(data => {
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
    console.log(this.employeeForm);

    if (this.employeeForm.invalid) {
      this.checkInvalidControls(this.employeeForm);
      this.constantService.markFormGroupTouched(this.employeeForm);
      return;
    }

    this.isLoading = true;

    let _employeeForm: any = {};
    _employeeForm = Object.assign(_employeeForm, this.employeeForm.value);

    let cnicIssuanceDate = this.constantService.formatDate(this.employeeForm.get('cnicIssuanceDate')?.value);
    _employeeForm['cnicIssuanceDate'] = cnicIssuanceDate;

    let cnicExpiryDate = this.constantService.formatDate(this.employeeForm.get('cnicExpiryDate')?.value);
    _employeeForm['cnicExpiryDate'] = cnicExpiryDate;

    let joinDate = this.constantService.formatDate(this.employeeForm.get('joinDate')?.value);
    _employeeForm['joinDate'] = joinDate;

    let dateOfConfirmation = this.constantService.formatDate(this.employeeForm.get('dateOfConfirmation')?.value);
    _employeeForm['dateOfConfirmation'] = dateOfConfirmation;

    let dateOfBirth = this.constantService.formatDate(this.employeeForm.get('dateOfBirth')?.value);
    _employeeForm['dateOfBirth'] = dateOfBirth;


    if (_employeeForm.id === 0) {
      (await this.employeeService.register(_employeeForm)).subscribe({
        next: (data) => {
          this.handleResponse(data);
        },
        error: (error) => {
          this.notificationsService.showNotification(error, 'snack-bar-danger');
          this.isLoading = false;
        }
      });
    } else {
      // _employeeForm['Password'] = '';
      this.employeeService.updateUser(_employeeForm).subscribe({
        next: (data) => {
          this.handleResponse(data);
        },
        error: (error) => {
          // Check if the error response contains validation errors
          // if (error.error && error.error.errors) {
          //   const errors = error.error.errors;
          //   let errorMessages = '';

          //   // Loop through each error key and combine messages
          //   for (const key in errors) {
          //     if (errors.hasOwnProperty(key)) {
          //       errorMessages += `${key}: ${errors[key].join(', ')}\n`;
          //     }
          //   }

          //   // Show formatted error messages
          //   this.notificationsService.showNotification(errorMessages, 'snack-bar-danger');
          // } else {
          // Fallback for generic errors
          this.notificationsService.showNotification('An unexpected error occurred.', 'snack-bar-danger');
          // }

          this.isLoading = false;
        }
      });
    }
  }

  checkInvalidControls(formGroup: FormGroup) {
    // Loop through each control in the FormGroup
    Object.keys(formGroup.controls).forEach(controlName => {
      const control = formGroup.get(controlName);

      // Check if the control is invalid
      if (control && control.invalid) {
        console.log(`Control '${controlName}' is invalid.`);
        this.notificationsService.showNotification(controlName + ' is invalid!', 'snack-bar-danger');
        // You can further log the specific errors for each control
        console.log(control.errors);
      }
    });

    // If there are FormArrays, check their controls as well
    if (formGroup instanceof FormArray) {
      formGroup.controls.forEach((formControl, index) => {
        if (formControl.invalid) {
          console.log(`FormArray control at index ${index} is invalid.`);
          this.notificationsService.showNotification(formControl + ' is invalid!', 'snack-bar-danger');
          console.log(formControl.errors);
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
    var companyId = this.employeeForm.get('companyId')?.value;
    this.departmentService.getDepartmentByCompany(companyId).subscribe(data => {
      this.departmentList = data;
    });
  }

  getStoreList(): void {
    var companyId = this.employeeForm.get('companyId')?.value;
    this.storeService.getStoreByCompany(companyId, false).subscribe(data => {
      this.storeList = data;
    });
  }

  getprojectList() {
    let _projectFilter: any = {};
    this.projectService.getAllProjects(_projectFilter).subscribe((data: any) => {
      this.projectList = data.item1;
    });
  }


checkRole(event: any) {
    const employeeRoleId = '46684446-1139-4085-6401-08ddf519c24b'; // Employee role ID
    let selectedIds = event.value as string[];

    // Check if Employee role is not included, and if so, add it
    if (!selectedIds.includes(employeeRoleId)) {
      selectedIds = [...selectedIds, employeeRoleId];
      this.employeeForm.get('roleId')?.setValue(selectedIds, { emitEvent: false });
    }

    // Map IDs to their respective names
    const selectedRoleNames = this.rolesList
      .filter((role: { id: any; }) => selectedIds.includes(role.id)) // Find roles that match selected IDs
      .map((role: { name: any; }) => role.name); // Extract the role names

    // Check if Admin is selected
    this.isAdmin = selectedRoleNames.includes('Admin');
  }

  // checkRole(event: any) {
  //   // Assuming event contains the array of selected role IDs
  //   const selectedIds = event.value; // This gives the selected role IDs (array)

  //   // Map IDs to their respective names
  //   const selectedRoleNames = this.rolesList
  //     .filter((role: { id: any; }) => selectedIds.includes(role.id)) // Find roles that match selected IDs
  //     .map((role: { name: any; }) => role.name); // Extract the role names

  //   if (selectedRoleNames[0] == 'Admin')
  //     this.isAdmin = true;
  //   console.log('Selected Role Names:', selectedRoleNames);
  //   // You can assign `selectedRoleNames` to a variable or use it as needed
  // }

  getDepartment() {
    var departmentId = this.employeeForm.get('departmentId')?.value;
    if (departmentId != "") {
      var selecteddept = this.departmentList.filter((element: any) => {
        return element.id == departmentId;
      })
      this.department = selecteddept[0].name ?? '';
    }
    return '';
  }

  onFileChange(event: any) {
    const reader = new FileReader();
    if (event.target.files && event.target.files.length) {
      const [file] = event.target.files;
      reader.readAsDataURL(file);

      reader.onload = () => {
        this.imageSrc = reader.result as string;

        this.employeeForm.get('imageName')?.patchValue(file.name);
        this.employeeForm.get('fileSource')?.patchValue(reader.result);
        this.employeeForm.get('extension')?.patchValue(file.name.split('.').pop().toLowerCase());
      };
    }
  }

  onFileSourceRemove(event: any) {
    this.employeeForm
      .get('imageName')?.patchValue('');

    this.employeeForm
      .get('fileSource')?.patchValue('');

    this.imageSrc = '';
  }

  getEmployeeDesignationList(): void {
    let _filterForm = {};
    this.employeeDesignationService.getAllEmployeeDesignations(_filterForm).subscribe(data => {
      this.employeeDesignationList = data.item1;
    });
  }

  getEmployeeEducationList(): void {
    let _filterForm = {};
    this.employeeEducationService.getAllEmployeeEducations(_filterForm).subscribe(data => {
      this.employeeEducationList = data.item1;
    });
  }

  getEmployeeGradeList(): void {
    let _filterForm = {};
    this.employeeGradeService.getAllEmployeeGrades(_filterForm).subscribe(data => {
      this.employeeGradeList = data.item1;
    });
  }

  getEmployeeShiftList(): void {
    let _filterForm = {};
    this.employeeShiftService.getAllEmployeeShifts(_filterForm).subscribe(data => {
      this.employeeShiftList = data.item1;
    });
  }

  getEmployeeTypeList(): void {
    let _filterForm = {};
    this.employeeTypeService.getAllEmployeeTypes(_filterForm).subscribe(data => {
      this.employeeTypeList = data.item1;
    });
  }

  getEmployeeBankList(): void {
    let _filterForm = {};
    this.employeeBankService.getAllEmployeeBanks(_filterForm).subscribe(data => {
      this.employeeBankList = data.item1;
    });
  }

  getEmployeeLeaveGroupsList(): void {
    let _filterForm = {};
    this.employeeLeaveGroupService.getAllEmployeeLeaveGroups(_filterForm).subscribe(data => {
      this.employeeLeaveGroupList = data.item1;
      this.getLeaves();
    });
  }

  getEmployeeDocumentsList(): void {
    const _filterForm = {};
    this.employeeDocumentTypeService.getAllEmployeeDocumentTypes(_filterForm).subscribe(data => {
      this.employeeDocumentTypeList = data.item1;

      const formArray = this.documentsFormArray;

      if (this.data?.element) {
        // EDIT MODE
        this.employeeDocumentTypeService.getEmployeeDocumentByEmployeeId(this.data.element.id).subscribe(savedDocs => {
          this.employeeDocumentTypeList.forEach((type: { id: any; }) => {
            const existingDoc = savedDocs.find((doc: any) => doc.employeeDocumentTypeId === type.id);
            formArray.push(this.formBuilder.group({
              employeeDocumentTypeId: [type.id],
              imageName: [existingDoc?.name || ''],
              fileSource: [existingDoc?.name || ''], // replace with actual base64 or path
              extension: [existingDoc?.name || '']
            }));
          });
        });
      } else {
        // ADD MODE
        this.employeeDocumentTypeList.forEach((type: { id: any; }) => {
          formArray.push(this.formBuilder.group({
            employeeDocumentTypeId: [type.id],
            imageName: [''],
            fileSource: [''],
            extension: ['']
          }));
        });
      }
    });
  }

  onEmpFileChange(event: any, index: number) {
    // const file = event.target.files[0];
    // const control = (this.employeeForm.get('documents') as FormArray).at(index);
    // control.patchValue({ file: file });

    const reader = new FileReader();
    if (event.target.files && event.target.files.length) {
      const [file] = event.target.files;
      reader.readAsDataURL(file);

      reader.onload = () => {
        this.imageSrc = reader.result as string;

        const control = (this.employeeForm.get('documents') as FormArray).at(index);
        control.patchValue({ imageName: file.name });
        control.patchValue({ fileSource: reader.result });
        control.patchValue({ extension: file.name.split('.').pop().toLowerCase() });
      };
    }
  }

  getLeaves() {
    debugger
    let employeeLeaveGroupId = this.employeeForm.get('employeeLeaveGroupId')?.value;

    // this.selectedLeaveGroup = this.employeeLeaveGroupList?.filter((x: any) => x.id === employeeLeaveGroupId)[0]
    // .employeeGroupLeaveType[0].employeeGroupLeaveTypeDetail;

    const today = new Date();

    const selectedLeaveGroup = this.employeeLeaveGroupList?.find((x: any) => x.id === employeeLeaveGroupId).employeeGroupLeaveType;

    if (selectedLeaveGroup) {
      const validLeaveType = selectedLeaveGroup.find((y: any) => {
        const startDate = new Date(y.hrYear.startDate);
        const endDate = new Date(y.hrYear.endDate);

        // Ensure we compare the date parts only (ignoring time)
        return today >= startDate && today <= endDate;
      });

      // If a valid leave type is found, extract its details
      if (validLeaveType && validLeaveType.employeeGroupLeaveTypeDetail.length) {
        this.selectedLeaveGroup = validLeaveType.employeeGroupLeaveTypeDetail;
      }
      else {
        this.selectedLeaveGroup = [];
      }
    }


  }

  getCityList(): void {
    let _filterForm = {};
    this.cityService.getAllCities(_filterForm).subscribe(data => {
      this.cityList = data.item1;
    });
  }

  handleResignValidation() {
    this.employeeForm.get('isResigned')?.valueChanges.subscribe((isResigned) => {
      const resignDateControl = this.employeeForm.get('resignDate');

      if (isResigned) {
        resignDateControl?.setValidators(Validators.required);
      } else {
        resignDateControl?.clearValidators();
        resignDateControl?.setValue(null); // optional: reset field when not required
      }

      resignDateControl?.updateValueAndValidity();
    });
  }

  getEmployeeOvertimeRateList(): void {
    let _filterForm = {};
    this.employeeOvertimeRateService.getAllEmployeeOvertimeRates(_filterForm).subscribe(data => {
      this.employeeOvertimeRateList = data.item1;
    });
  }

    getEmployeeSiteTypeList(): void {
    let _filterForm = {};
    this.employeeWorkSiteTypeService.getAllEmployeeWorkSiteTypes(_filterForm).subscribe(data => {
      this.employeeSiteTypeList = data.item1;
    });
  }

  handleDepartmentValidation() {
    this.employeeForm.get('departmentId')?.valueChanges.subscribe((departmentId) => {
      const projectIdControl = this.employeeForm.get('projectIds');
      this.department = this.departmentList?.filter((x: { id: any; }) => x.id === departmentId)[0].name ?? '';
      if (this.department == 'Store') {
        projectIdControl?.setValidators(Validators.required);
      } else {
        projectIdControl?.clearValidators();
        projectIdControl?.setValue(null); // optional: reset field when not required
      }

      projectIdControl?.updateValueAndValidity();
    });
  }

   onSiteTypeChange(event: MatSelectChange): void {
      this.employeeForm.patchValue({
      isMobileDeviceRegister: false,
      isAvailableForMobile: false,
      isAvailableForWeb: false,
      isDistCompForAtten: false
    });
  }
}