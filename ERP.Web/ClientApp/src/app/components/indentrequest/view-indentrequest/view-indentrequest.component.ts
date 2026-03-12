import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-indentrequest',
    templateUrl: './view-indentrequest.component.html',
    styleUrl: './view-indentrequest.component.css',
    standalone: false
})

export class ViewIndentrequestComponent {
  indentrequestForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;

  constructor(private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.indentrequestForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      name: ['', Validators.required],
      description: ['', Validators.required],
      categoryId: ['', Validators.required],
      subCategoryId: ['', Validators.required],
      indentrequestTypeId: ['', Validators.required],
      uomId: ['', Validators.required],
      recordLevel: [0, Validators.required],
      leadTime: [0, Validators.required],
      rate : [0, Validators.required],
      weight : [0, Validators.required],
      length : [0, Validators.required],
      height : [0, Validators.required],
      width : [0, Validators.required],
      model : [0, Validators.required],
      make : [0, Validators.required],
      excessQtyPer : [0, Validators.required],
      companyId: [0],
    });
    
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (this.data.element.id != null) {
      this.isEditMode = true;
    }
    this.constantService.LoadData(element, this.indentrequestForm);
  }
}
