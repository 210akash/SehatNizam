import { Component, Inject } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { EmployeeLeaveService } from '../../employee-leave/employee-leave.service';
import { EmployeeService } from '../../employee/employee.service';
import { DepartmentService } from '../../../department/department.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';

@Component({
  selector: 'app-add-manage-employee-leave',
  templateUrl: './add-manage-employee-leave.component.html',
  styleUrl: './add-manage-employee-leave.component.css',
  standalone: false
})

export class AddManageEmployeeLeaveComponent {
  employeeLeaveForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  leaveBalance: any;
  departmentList : any;
  employeeList : any;
  constructor(private employeeService: EmployeeService,private departmentService: DepartmentService, private dialog: MatDialog, private notificationsService: NotificationsService,
    private formBuilder: FormBuilder, private employeeLeaveService: EmployeeLeaveService, private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.employeeLeaveForm = this.formBuilder.group({
      employeeId: [0],
      employeeName: [''],
      employee: [''],
      departmentId: [0],
      startDate: [null, Validators.required],
      endDate: [null, Validators.required],
      reason: ['', Validators.required],
      isFirstHalfDay: [false],
      isLastHalfDay: [false],
      employeeGroupLeaveTypeDetailId: ['', Validators.required],
      noOfDays: [0, Validators.required]
    }, { validators: this.dateRangeValidator });

    //this.LoadData(this.data.element);
    

    // Ensure only one half-day checkbox is selected at a time
    // this.employeeLeaveForm.get('isFirstHalfDay')?.valueChanges.subscribe((checked: boolean) => {
    //   if (checked) {
    //     this.employeeLeaveForm.patchValue({ isLastHalfDay: false }, { emitEvent: false });
    //   }
    // });

    // this.employeeLeaveForm.get('isLastHalfDay')?.valueChanges.subscribe((checked: boolean) => {
    //   if (checked) {
    //     this.employeeLeaveForm.patchValue({ isFirstHalfDay: false }, { emitEvent: false });
    //   }
    // });

    // Listen to form changes to calculate the number of days
    this.employeeLeaveForm.valueChanges.subscribe(() => {
      this.calculateDays();
      this.checkLeaveBalance();
    });
    this.getDepartmentList();
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.employeeLeaveForm);
    }
  }

  SaveData() {
    if (this.employeeLeaveForm.invalid) {
      this.constantService.markFormGroupTouched(this.employeeLeaveForm);
      return;
    }

    this.isLoading = true;
    let _leaveForm: any = {};
    _leaveForm = Object.assign(_leaveForm, this.employeeLeaveForm.value);

    this.employeeLeaveService.saveEmployeeLeaveByHr(_leaveForm).subscribe({
      next: (data) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else
          this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        console.error(error);
        this.isLoading = false;
      }
    });
  }

  getEmployeeLeaveBalance(employeeId:any): void {
    this.employeeLeaveService.getLeaveBalanceByEmployee(employeeId).subscribe(data => {
      this.leaveBalance = data;
    });
  }

  dateRangeValidator(control: AbstractControl) {
    const startDate = control.get('startDate')?.value;
    const endDate = control.get('endDate')?.value;

    if (startDate && endDate && startDate > endDate) {
      this.notificationsService.showNotification("From Date can not be greater!", 'snack-bar-danger');
      return { startDateAfterToDate: true };
    }

    return null;
  }

  calculateDays() {
    const startDate = this.employeeLeaveForm.get('startDate')?.value;
    const endDate = this.employeeLeaveForm.get('endDate')?.value;

    if (startDate && endDate) {
      const diffTime = Math.abs(new Date(endDate).getTime() - new Date(startDate).getTime());
      const days = Math.ceil(diffTime / (1000 * 3600 * 24)) + 1;

      const currentNoOfDays = this.employeeLeaveForm.get('noOfDays')?.value;

      if (currentNoOfDays !== days) {
        this.employeeLeaveForm.patchValue({ noOfDays: days }, { emitEvent: false });
      }
    } else {
      this.employeeLeaveForm.patchValue({ noOfDays: 0 }, { emitEvent: false });
    }
  }

  checkLeaveBalance() {
    const leaveTypeId = this.employeeLeaveForm.get('employeeGroupLeaveTypeDetailId')?.value;
    const noOfDays = this.employeeLeaveForm.get('noOfDays')?.value;

    if (leaveTypeId) {
      const selectedLeave = this.leaveBalance.find((item: any) => item.id === leaveTypeId);
      if (selectedLeave && noOfDays > selectedLeave.balance) {
        this.employeeLeaveForm.get('noOfDays')?.setErrors({ insufficientBalance: true });
        this.notificationsService.showNotification('Leaves applied are greater than limit!', 'snack-bar-danger');
      } else {
        this.employeeLeaveForm.get('noOfDays')?.setErrors(null);
      }
    }
  }

  getDepartmentList(): void {
    this.departmentService.getDepartmentByCompany('2').subscribe(data => {
      this.departmentList = data;
    });
  }

  getEmployeeList(event: any) {
    var filter = event.currentTarget.value;
    var departmentId = this.employeeLeaveForm.get('departmentId')?.value;
    var getEmployeeFilter  = {
      name : filter,
      departmentId : departmentId
    }
    this.employeeService.getEmployeeByName(getEmployeeFilter)
        .subscribe((data: any) => {
            this.employeeList = data;
        });
}

  onOptionSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedValue = event.option.value;
    if (!selectedValue) {
      console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');
      return;
    }
      // Get the selected item details from your getaccount method
      const selectedItem = this.getemployee(selectedValue.id);
      if (!selectedItem) {
        console.error('Selected item not found.');
        return;
      }

      // Patch the values into the form group
        this.employeeLeaveForm.get('employeeId')?.patchValue(selectedValue.id);
        this.employeeLeaveForm.get('employeeName')?.patchValue(selectedValue?.hrCode +' : '+  selectedValue?.firstName +' '+ selectedValue?.lastName +' ('+ selectedValue?.designation + ')');
        this.employeeLeaveForm.get('employee')?.patchValue(selectedValue);
        this.getEmployeeLeaveBalance(selectedValue.id);
  }

  getemployee(itemId: string) {
    return this.employeeList.find((option: { id: string; }) => option.id === itemId);
  }
  
  onInputCleared(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    if (!inputValue.trim()) {
        this.employeeLeaveForm.get('id')?.patchValue(0);
        this.employeeLeaveForm.get('employeeId')?.patchValue(0);
        this.employeeLeaveForm.get('employeeName')?.patchValue('');
        this.employeeLeaveForm.get('employee')?.patchValue('');
    }
  }

 reset(){
   this.employeeLeaveForm.get('id')?.patchValue(0);
        this.employeeLeaveForm.get('employeeId')?.patchValue(0);
        this.employeeLeaveForm.get('employeeName')?.patchValue('');
        this.employeeLeaveForm.get('employee')?.patchValue('');
        this.employeeList = null;
 }
}