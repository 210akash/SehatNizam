import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { CandidateEvaluationCategoryService } from '../candidateevaluationcategory.service';

@Component({
    selector: 'app-add-candidateevaluationcategory',
    templateUrl: './add-candidateevaluationcategory.component.html',
    styleUrl: './add-candidateevaluationcategory.component.css',
    standalone: false
})

export class AddCandidateEvaluationCategoryComponent {
  candidateevaluationcategoryForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private candidateevaluationcategoryService: CandidateEvaluationCategoryService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.candidateevaluationcategoryForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
      description: ['', Validators.required]
    });
    
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.candidateevaluationcategoryForm);
    }
  }

  SaveData() {
    if (this.candidateevaluationcategoryForm.invalid) {
      this.constantService.markFormGroupTouched(this.candidateevaluationcategoryForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.candidateevaluationcategoryForm.value);

    this.candidateevaluationcategoryService.saveCandidateEvaluationCategory(_clienttemperatureForm).subscribe({
      next: (data) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else
          this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        console.error(error);
        this.isLoading = false;
      }
    });
  }
}
