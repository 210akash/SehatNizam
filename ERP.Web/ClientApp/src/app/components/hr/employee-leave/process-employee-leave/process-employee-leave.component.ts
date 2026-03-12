import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { EmployeeLeaveService } from '../employee-leave.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-process-employee-leave',
  templateUrl: './process-employee-leave.component.html',
  styleUrl: './process-employee-leave.component.css',
  standalone: false
})

export class ProcessEmployeeLeaveComponent {
  processEmployeeLeaveForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;
  leaveBalance: any;
  noOfDays: number = 0;

  constructor(private employeeLeaveService: EmployeeLeaveService, private notificationsService: NotificationsService, private formBuilder: FormBuilder,
    private dialog: MatDialog, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.processEmployeeLeaveForm = this.formBuilder.group({
      id: [this.data.element.id],
      comments: ['']
    });

    this.getEmployeeLeaveBalance();
  }

  Process() {
    this.employeeLeaveService.processEmployeeLeave(this.processEmployeeLeaveForm.value).subscribe({
      next: (data) => {
        if (data == 200) {
          this.notificationsService.showNotification('Leave Processed Successfully!', 'snack-bar-success');
          this.dialog.closeAll();
        }
        else
          this.notificationsService.showNotification('Error Processing Leave!', 'snack-bar-danger');
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