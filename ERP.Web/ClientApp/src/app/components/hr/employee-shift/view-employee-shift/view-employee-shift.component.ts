import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
    selector: 'app-view-employee-shift',
    templateUrl: './view-employee-shift.component.html',
    styleUrl: './view-employee-shift.component.css',
    standalone: false
})

export class ViewEmployeeShiftComponent {
  employeeShiftForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;

  constructor(private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.employeeShiftForm = this.formBuilder.group({
      code: ['', Validators.required],
      name: ['', Validators.required],
      fromTime: ['', Validators.required],
      toTime: ['', Validators.required],
      isDualDate: [false],
    });
    
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (this.data.element.id != null) {
      this.isEditMode = true;
    }
    this.constantService.LoadData(element, this.employeeShiftForm);
  }
}
