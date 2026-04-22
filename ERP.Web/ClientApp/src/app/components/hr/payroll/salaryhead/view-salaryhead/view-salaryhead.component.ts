import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConstantService, SalaryHeadTypeEnum } from '../../../../../Service/constant.service';

@Component({
    selector: 'app-view-salaryhead',
    templateUrl: './view-salaryhead.component.html',
    styleUrl: './view-salaryhead.component.css',
    standalone: false
})

export class ViewSalaryHeadComponent {
  salaryheadForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;
  salaryHeadTypes: { [key: number]: string } = {};
  constructor(private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
        this.salaryHeadTypes = Object.keys(SalaryHeadTypeEnum)
              .filter(key => isNaN(Number(key))) // Filter out numeric keys
              .reduce((acc, key) => {
                const value = SalaryHeadTypeEnum[key as keyof typeof SalaryHeadTypeEnum];
                acc[value] = key; // Map numeric value to string name
                return acc;
              }, {} as { [key: number]: string });
    this.salaryheadForm = this.formBuilder.group({
      name: ['', Validators.required],
    });
    
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (this.data.element.id != null) {
      this.isEditMode = true;
    }
    this.constantService.LoadData(element, this.salaryheadForm);
  }
}
