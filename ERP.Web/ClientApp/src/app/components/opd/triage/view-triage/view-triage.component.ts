import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-view-triage',
  templateUrl: './view-triage.component.html',
  styleUrls: ['./view-triage.component.css'],standalone: false
})
export class ViewTriageComponent implements OnInit {
  viewTriageForm!: FormGroup;

  constructor(
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.viewTriageForm = this.formBuilder.group({
      id: [0],
      appointmentId: [''],
      triageCategoryId: [''],
      triagePriorityId: [''],
      sugarTypeId: [''],
      temperature: [''],
      pulse: [''],
      systolicBp: [''],
      diastolicBp: [''],
      spo2: [''],
      weight: [''],
      heightFeet: [''],
      heightInches: [''],
      heightCm: [''],
      bmi: [''],
      bloodSugar: [''],
      chiefComplaint: [''],
      allergies: [''],
      medications: [''],
      notes: [''],
      triageScore: ['']
    });

    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.viewTriageForm);
  }
}
