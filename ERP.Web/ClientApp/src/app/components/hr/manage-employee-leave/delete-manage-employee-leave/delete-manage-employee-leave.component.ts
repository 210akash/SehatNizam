import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { EmployeeLeaveService } from '../../employee-leave/employee-leave.service';

@Component({
    selector: 'app-delete-manage-employee-leave',
    templateUrl: './delete-manage-employee-leave.component.html',
    styleUrl: './delete-manage-employee-leave.component.css',
    standalone: false
})

export class DeleteManageEmployeeLeaveComponent {
  employeeLeaveForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog, private formBuilder: FormBuilder, private notificationsService: NotificationsService, private employeeLeaveService: EmployeeLeaveService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.employeeLeaveForm = this.formBuilder.group({
      id: [0],
      bankName: ['', Validators.required],
      branchCode: ['', Validators.required],
      branchName: ['', Validators.required],
    });
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (this.data.element.id != null) {
      this.isEditMode = true;
    }
    this.constantService.LoadData(element, this.employeeLeaveForm);
  }

  async delete() {
    (await this.employeeLeaveService.deleteEmployeeLeave(this.data.element.id)).subscribe({
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
