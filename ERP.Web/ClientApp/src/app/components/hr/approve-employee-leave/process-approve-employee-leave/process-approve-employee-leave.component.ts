import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { EmployeeLeaveService } from '../../employee-leave/employee-leave.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-process-approve-employee-leave',
  templateUrl: './process-approve-employee-leave.component.html',
  styleUrl: './process-approve-employee-leave.component.css',
  standalone: false
})

export class ProcessApproveEmployeeLeaveComponent {
  processEmployeeLeaveForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;
  leaveBalance: any;
  noOfDays: number = 0;

  constructor(private notificationsService: NotificationsService, private employeeLeaveService: EmployeeLeaveService, private formBuilder: FormBuilder, private dialog: MatDialog, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.processEmployeeLeaveForm = this.formBuilder.group({
      id: [this.data.element.id],
      comments: ['']
    });
    this.getEmployeeLeaveBalance(this.data.element.employeeId);
  }

  Approve() {
    this.employeeLeaveService.managerApproveLeave(this.processEmployeeLeaveForm.value).subscribe({
      next: (data) => {
        if (data == 200) {
          this.notificationsService.showNotification('Leave Approve Successfully!', 'snack-bar-success');
          this.dialog.closeAll();
        }
        else
          this.notificationsService.showNotification('Error Approve Leave!', 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        console.error(error);
        this.isLoading = false;
      }
    });
  }

  Reject() {
    this.employeeLeaveService.rejectEmployeeLeave(this.processEmployeeLeaveForm.value).subscribe({
      next: (data) => {
        if (data == 200) {
          this.notificationsService.showNotification('Leave Reject Successfully!', 'snack-bar-success');
          this.dialog.closeAll();
        }
        else
          this.notificationsService.showNotification('Error Reject Leave!', 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        console.error(error);
        this.isLoading = false;
      }
    });
  }

  getEmployeeLeaveBalance(employeeId: any): void {
    this.employeeLeaveService.getLeaveBalanceByEmployee(employeeId).subscribe(data => {
      this.leaveBalance = data;
      this.calculateDays();
    });
  }

  calculateDays() {
    const start = new Date(this.data.element.startDate.split('T')[0]);
    const end = new Date(this.data.element.endDate.split('T')[0]);

    const diffTime = Math.abs(end.getTime() - start.getTime());
    const days = Math.ceil(diffTime / (1000 * 3600 * 24)) + 1;

    this.noOfDays = days;
  }
}