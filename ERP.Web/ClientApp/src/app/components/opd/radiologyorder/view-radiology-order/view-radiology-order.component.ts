// view-radiology-order.component.ts
import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-view-radiology-order',
  templateUrl: './view-radiology-order.component.html',
  styleUrls: ['./view-radiology-order.component.css'],
  standalone: false
})
export class ViewRadiologyOrderComponent implements OnInit {
confirmRadiologyOrder() {
throw new Error('Method not implemented.');
}
  form!: FormGroup;
  currentDate: Date = new Date();

  constructor(
    private fb: FormBuilder, 
    private constantService: ConstantService, 
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({ 
      id: [''], 
      appointmentId: [''], 
      radiologyOrderTypeId: [''], 
      statusId: ['']
    });
    this.constantService.LoadData(this.data.element, this.form);
  }

  getStatusName(statusId: number): string {
    const statusMap: { [key: number]: string } = {
      1: 'Pending',
      2: 'Confirmed',
      3: 'In Progress',
      4: 'Completed',
      5: 'Cancelled'
    };
    return statusMap[statusId] || '-';
  }
    printDocument(): void {
    const printContent = document.getElementById('printDoc');
    if (printContent) {
      const originalContents = document.body.innerHTML;
      const printHTML = printContent.innerHTML;
      
      document.body.innerHTML = printHTML;
      window.print();
      document.body.innerHTML = originalContents;
    }
  }
}