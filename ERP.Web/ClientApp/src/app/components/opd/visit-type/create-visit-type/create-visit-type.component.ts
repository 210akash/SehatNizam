import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { VisitTypeService } from '../visit-type.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-create-visit-type',
  templateUrl: './create-visit-type.component.html',
  styleUrls: ['./create-visit-type.component.css'],standalone: false
})

export class CreateVisitTypeComponent implements OnInit {
  createVisitTypeForm!: FormGroup;
  isLoading = false;
  visitTypeListFilerForm!: FormGroup;
  dataSource!: any;
  roleList: any;
  dropdownSettings: any;
  isEditMode: boolean = false;
  pageSize = 1000;
  currentPage = 0;

  constructor(private notificationsService: NotificationsService, private dialog: MatDialog, private formBuilder: FormBuilder, private constantService: ConstantService, private visitTypeService: VisitTypeService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.createVisitTypeForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
    });

    this.LoadData(this.data.element);
  }

  get f() {
    return this.createVisitTypeForm.controls;
  }

  async saveVisitType() {
    this.isLoading = true;
    if (this.createVisitTypeForm.invalid) {
      this.constantService.markFormGroupTouched(this.createVisitTypeForm);
      return;
    }
    let _createVisitTypeForm: any = {};
    _createVisitTypeForm = Object.assign(_createVisitTypeForm, this.createVisitTypeForm.value);

    (await this.visitTypeService.saveVisitType(_createVisitTypeForm)).subscribe(
      {
        next: (data: { Status: number; }) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('Visit Type Saved Successfully!', 'snack-bar-success');
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
      this.constantService.LoadData(element, this.createVisitTypeForm);
    }
    console.log(this.createVisitTypeForm);
  }


}