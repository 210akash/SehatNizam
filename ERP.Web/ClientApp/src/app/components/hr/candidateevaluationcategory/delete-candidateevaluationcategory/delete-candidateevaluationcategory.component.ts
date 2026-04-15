import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { CandidateEvaluationCategoryService } from '../candidateevaluationcategory.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
    selector: 'app-delete-candidateevaluationcategory',
    templateUrl: './delete-candidateevaluationcategory.component.html',
    styleUrl: './delete-candidateevaluationcategory.component.css',
    standalone: false
})

export class DeleteCandidateEvaluationCategoryComponent {
  candidateevaluationcategoryForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog, private formBuilder: FormBuilder, private notificationsService: NotificationsService, private candidateevaluationcategoryService: CandidateEvaluationCategoryService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.candidateevaluationcategoryForm = this.formBuilder.group({
      id: [0],
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

  async delete() {
    (await this.candidateevaluationcategoryService.deleteCandidateEvaluationCategory(this.data.element.id)).subscribe({
      next: (data) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        }
      },
      error: (error) => {
        console.log(error);
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }
}
