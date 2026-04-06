import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { TriageCategoryService } from '../triage-category.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-create-triage-category',
  templateUrl: './create-triage-category.component.html',
  styleUrls: ['./create-triage-category.component.css'],standalone: false
})

export class CreateTriageCategoryComponent implements OnInit {
  createTriageCategoryForm!: FormGroup;
  isLoading = false;
  appointmentTypeListFilerForm!: FormGroup;
  dataSource!: any;
  roleList: any;
  dropdownSettings: any;
  isEditMode: boolean = false;
  pageSize = 1000;
  currentPage = 0;

  constructor(private notificationsService: NotificationsService, private dialog: MatDialog, private formBuilder: FormBuilder, private constantService: ConstantService, private appointmentTypeService: TriageCategoryService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.createTriageCategoryForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
      description: ['', Validators.required]
    });

    this.LoadData(this.data.element);
  }

  get f() {
    return this.createTriageCategoryForm.controls;
  }

  async saveTriageCategory() {
    this.isLoading = true;
    if (this.createTriageCategoryForm.invalid) {
      this.constantService.markFormGroupTouched(this.createTriageCategoryForm);
      return;
    }
    let _createTriageCategoryForm: any = {};
    _createTriageCategoryForm = Object.assign(_createTriageCategoryForm, this.createTriageCategoryForm.value);

    (await this.appointmentTypeService.saveTriageCategory(_createTriageCategoryForm)).subscribe(
      {
        next: (data: { Status: number; }) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('Appointment Type Saved Successfully!', 'snack-bar-success');
            this.dialog.closeAll();
            this.isLoading = false;
          }
          else if (data.Status == 409) {
            this.notificationsService.showNotification('Name already exist!', 'snack-bar-danger');
            this.isLoading = false;
          }
        },
        error: (error: any) => {
          this.notificationsService.showNotification('Please Fill the required fields!', 'snack-bar-danger');
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  LoadData(element: any) {
    if (this.data.element?.id != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.createTriageCategoryForm);
    }
    console.log(this.createTriageCategoryForm);
  }


}