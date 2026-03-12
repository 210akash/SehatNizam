import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { EmployeeLeaveGroupService } from '../employee-leave-group.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
    selector: 'app-delete-employee-leave-group',
    templateUrl: './delete-employee-leave-group.component.html',
    styleUrl: './delete-employee-leave-group.component.css',
    standalone: false
})

export class DeleteEmployeeLeaveGroupComponent {
  employeeLeaveGroupForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog, private formBuilder: FormBuilder, private notificationsService: NotificationsService, private employeeLeaveGroupService: EmployeeLeaveGroupService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.employeeLeaveGroupForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
    });
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (this.data.element.id != null) {
      this.isEditMode = true;
    }
    this.constantService.LoadData(element, this.employeeLeaveGroupForm);
  }

  async delete() {
    (await this.employeeLeaveGroupService.deleteEmployeeLeaveGroup(this.data.element.id)).subscribe({
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
