import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-item',
    templateUrl: './view-item.component.html',
    styleUrl: './view-item.component.css',
    standalone: false
})

export class ViewItemComponent {
  itemForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;

  constructor(private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.itemForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      name: ['', Validators.required],
      description: ['', Validators.required],
      categoryId: ['', Validators.required],
      subCategoryId: ['', Validators.required],
      itemTypeId: ['', Validators.required],
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
    this.constantService.LoadData(element, this.itemForm);
  }
}
