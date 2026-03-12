import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { EmployeeLeaveService } from '../../employee-leave/employee-leave.service';

@Component({
    selector: 'app-view-manage-employee-leave',
    templateUrl: './view-manage-employee-leave.component.html',
    styleUrl: './view-manage-employee-leave.component.css',
    standalone: false
})

export class ViewManageEmployeeLeaveComponent {
  employeeLeaveForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;
  leaveBalance: any;
  noOfDays: number = 0;
  constructor(private employeeLeaveService: EmployeeLeaveService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
this.getEmployeeLeaveBalance(this.data.element.employeeId);
  }

 getEmployeeLeaveBalance(employeeId:any): void {
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
