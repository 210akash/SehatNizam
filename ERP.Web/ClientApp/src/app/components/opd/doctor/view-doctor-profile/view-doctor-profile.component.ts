import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-view-doctor-profile',
  templateUrl: './view-doctor-profile.component.html',
  styleUrls: ['./view-doctor-profile.component.css'],
  standalone: false
})
export class ViewDoctorProfileComponent implements OnInit {
  form!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      doctorName: [''],
      pmdcNumber: [''],
      qualification: [''],
      experienceYears: [0],
      specialization: [''],
      consultationFee: [0],
      hospitalPercentage: [0],
      biography: [''],
      isAvailableForOPD: [false],
      isAvailableForIPD: [false]
    });
    this.constantService.LoadData(this.data.element, this.form);
  }
}
