import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { EmployeeWorkSiteTypeService } from '../employee-worksitetype.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
    selector: 'app-delete-employee-worksitetype',
    templateUrl: './delete-employee-worksitetype.component.html',
    styleUrl: './delete-employee-worksitetype.component.css',
    standalone: false
})

export class DeleteEmployeeWorkSiteTypeComponent {
  employeeWorkSiteTypeForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog, private formBuilder: FormBuilder, private notificationsService: NotificationsService, private employeeWorkSiteTypeService: EmployeeWorkSiteTypeService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.employeeWorkSiteTypeForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
    });
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (this.data.element.id != null) {
      this.isEditMode = true;
    }
    this.constantService.LoadData(element, this.employeeWorkSiteTypeForm);
  }

  async delete() {
    (await this.employeeWorkSiteTypeService.deleteEmployeeWorkSiteType(this.data.element.id)).subscribe({
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
