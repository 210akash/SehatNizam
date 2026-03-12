import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
    selector: 'app-view-employee-bank',
    templateUrl: './view-employee-bank.component.html',
    styleUrl: './view-employee-bank.component.css',
    standalone: false
})

export class ViewEmployeeBankComponent {
  employeeBankForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;

  constructor(private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.employeeBankForm = this.formBuilder.group({
      name: ['', Validators.required],
      bankName: ['', Validators.required],
      branchCode: ['', Validators.required],
      branchName: ['', Validators.required],
    });
    
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (this.data.element.id != null) {
      this.isEditMode = true;
    }
    this.constantService.LoadData(element, this.employeeBankForm);
  }
}
