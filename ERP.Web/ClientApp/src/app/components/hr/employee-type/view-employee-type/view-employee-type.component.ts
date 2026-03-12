import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
    selector: 'app-view-employee-type',
    templateUrl: './view-employee-type.component.html',
    styleUrl: './view-employee-type.component.css',
    standalone: false
})

export class ViewEmployeeTypeComponent {
  employeeTypeForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;

  constructor(private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.employeeTypeForm = this.formBuilder.group({
      name: ['', Validators.required],
      noOfLeavesPerMonth: [0, Validators.required]
    });
    
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (this.data.element.id != null) {
      this.isEditMode = true;
    }
    this.constantService.LoadData(element, this.employeeTypeForm);
  }
}
