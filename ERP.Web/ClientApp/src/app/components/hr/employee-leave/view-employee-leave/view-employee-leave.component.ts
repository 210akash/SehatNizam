import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { EmployeeLeaveService } from '../employee-leave.service';

@Component({
  selector: 'app-view-employee-leave',
  templateUrl: './view-employee-leave.component.html',
  styleUrl: './view-employee-leave.component.css',
  standalone: false
})

export class ViewEmployeeLeaveComponent {
  isLoading = false;
  isEditMode: boolean = true;
  leaveBalance: any;
  noOfDays: number = 0;

  constructor(private employeeLeaveService: EmployeeLeaveService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.getEmployeeLeaveBalance();
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