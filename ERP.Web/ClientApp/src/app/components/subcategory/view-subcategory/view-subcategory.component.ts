import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-subcategory',
    templateUrl: './view-subcategory.component.html',
    styleUrl: './view-subcategory.component.css',
    standalone: false
})

export class ViewSubcategoryComponent {
  subcategoryForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;

  constructor(private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.subcategoryForm = this.formBuilder.group({
      code: ['', Validators.required],
      name: ['', Validators.required],
      description: ['', Validators.required],
    });
    
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (this.data.element.id != null) {
      this.isEditMode = true;
    }
    this.constantService.LoadData(element, this.subcategoryForm);
  }
}
