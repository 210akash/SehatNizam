import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { TriageCategoryService } from '../triage-category.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-delete-triage-category',
  templateUrl: './delete-triage-category.component.html',
  styleUrls: ['./delete-triage-category.component.css'],standalone: false
})

export class DeleteTriageCategoryComponent implements OnInit {
  deleteTriageCategoryForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  roleList: any;

  constructor(private notificationsService: NotificationsService, private dialog: MatDialog, private appointmentTypeService: TriageCategoryService, private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.deleteTriageCategoryForm = this.formBuilder.group({
      id: [0],
      name: [''],
    });

    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.deleteTriageCategoryForm);
  }

  async delete() {
    (await this.appointmentTypeService.deleteTriageCategory(this.data.element.id)).subscribe({
      next: (data: { Status: number; Message: string; }) => {
        if (data.Status == 200) {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Message, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Message, 'snack-bar-danger');
        }
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }


}
