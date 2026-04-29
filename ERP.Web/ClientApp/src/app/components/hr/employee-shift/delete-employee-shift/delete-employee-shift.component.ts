import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { EmployeeShiftService } from '../employee-shift.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
    selector: 'app-delete-employee-shift',
    templateUrl: './delete-employee-shift.component.html',
    styleUrl: './delete-employee-shift.component.css',
    standalone: false
})

export class DeleteEmployeeShiftComponent {
  employeeShiftForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog, private formBuilder: FormBuilder, private notificationsService: NotificationsService, private employeeShiftService: EmployeeShiftService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.employeeShiftForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      name: ['', Validators.required],
      fromTime: ['', Validators.required],
      toTime: ['', Validators.required],
      isDualDate: [false],
    });
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (this.data.element.id != null) {
      this.isEditMode = true;
    }
    this.constantService.LoadData(element, this.employeeShiftForm);
  }

  async delete() {
    (await this.employeeShiftService.deleteEmployeeShift(this.data.element.id)).subscribe({
      next: (data) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        }
      },
      error: (error) => {
        console.log(error);
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }
}
