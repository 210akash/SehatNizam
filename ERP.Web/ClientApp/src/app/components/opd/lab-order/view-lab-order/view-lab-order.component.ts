// view-lab-order.component.ts
import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-view-lab-order',
  templateUrl: './view-lab-order.component.html',
  styleUrls: ['./view-lab-order.component.css'],
  standalone: false
})
export class ViewLabOrderComponent implements OnInit {
  confirmLabOrder() {
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
      labOrderTypeId: [''],
      statusId: ['']
    });
    this.constantService.LoadData(this.data.element, this.form);
  }

  private getPatient(): any {
    const patient = this.data.element?.appointment?.patient ?? {};
    const master = patient.patientMaster ?? {};
    return {
      name: master.name ?? patient.name ?? '',
      mrn: patient.mrn ?? '',
      gender: master.gender ?? patient.gender ?? '',
      age: master.age ?? patient.age ?? '',
      phoneNo: master.phoneNo ?? patient.phoneNo ?? '',
      cnic: master.cnic ?? patient.cnic ?? '',
      address: master.address ?? patient.address ?? ''
    };
  }

  getPatientName(): string {
    return this.getPatient().name || '-';
  }

  getPatientMrn(): string {
    return this.getPatient().mrn || '-';
  }

  getPatientAge(): string {
    const age = this.getPatient().age;
    return age ? `${age} years` : '-';
  }

  getPatientGender(): string {
    return this.getPatient().gender || '-';
  }

  getPatientPhone(): string {
    return this.getPatient().phoneNo || '-';
  }

  getPatientCnic(): string {
    return this.getPatient().cnic || '-';
  }

  getPatientAddress(): string {
    return this.getPatient().address || '-';
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
