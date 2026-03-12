import { Component, Inject } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { EmployeeLeaveService } from '../employee-leave.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { CompanyService } from '../../../company/company.service';

@Component({
  selector: 'app-add-employee-leave',
  templateUrl: './add-employee-leave.component.html',
  styleUrl: './add-employee-leave.component.css',
  standalone: false
})

export class AddEmployeeLeaveComponent {
  employeeLeaveForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  leaveBalance: any;

  constructor(private companyService: CompanyService, private dialog: MatDialog, private notificationsService: NotificationsService,
    private formBuilder: FormBuilder, private employeeLeaveService: EmployeeLeaveService, private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.employeeLeaveForm = this.formBuilder.group({
      id: [0],
      startDate: [null, Validators.required],
      endDate: [null, Validators.required],
      reason: ['', Validators.required],
      isFirstHalfDay: [false],
      isLastHalfDay: [false],
      employeeGroupLeaveTypeDetailId: ['', Validators.required],
      noOfDays: [0, Validators.required]
    }, { validators: this.dateRangeValidator });

    this.LoadData(this.data.element);
    this.getEmployeeLeaveBalance();

    // Ensure only one half-day checkbox is selected at a time
    this.employeeLeaveForm.get('isFirstHalfDay')?.valueChanges.subscribe((checked: boolean) => {
      if (checked) {
        this.employeeLeaveForm.patchValue({ isLastHalfDay: false }, { emitEvent: false });
      }
    });

    this.employeeLeaveForm.get('isLastHalfDay')?.valueChanges.subscribe((checked: boolean) => {
      if (checked) {
        this.employeeLeaveForm.patchValue({ isFirstHalfDay: false }, { emitEvent: false });
      }
    });

    // Listen to form changes to calculate the number of days
    this.employeeLeaveForm.valueChanges.subscribe(() => {
      this.calculateDays();
      this.checkLeaveBalance();
    });
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
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.employeeLeaveForm.value);

    this.employeeLeaveService.saveEmployeeLeave(_clienttemperatureForm).subscribe({
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

  getEmployeeLeaveBalance(): void {
    this.employeeLeaveService.getEmployeeLeaveBalance().subscribe(data => {
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


}