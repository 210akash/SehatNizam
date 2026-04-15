import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
    selector: 'app-view-candidateevaluationcategory',
    templateUrl: './view-candidateevaluationcategory.component.html',
    styleUrl: './view-candidateevaluationcategory.component.css',
    standalone: false
})

export class ViewCandidateEvaluationCategoryComponent {
  candidateevaluationcategoryForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;

  constructor(private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.candidateevaluationcategoryForm = this.formBuilder.group({
      name: ['', Validators.required],
      description: ['', Validators.required]
    });
    
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (this.data.element.id != null) {
      this.isEditMode = true;
    }
    this.constantService.LoadData(element, this.candidateevaluationcategoryForm);
  }
}
