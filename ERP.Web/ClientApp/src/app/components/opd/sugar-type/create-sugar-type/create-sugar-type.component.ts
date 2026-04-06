import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { SugarTypeService } from '../sugar-type.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-create-sugar-type',
  templateUrl: './create-sugar-type.component.html',
  styleUrls: ['./create-sugar-type.component.css'],standalone: false
})

export class CreateSugarTypeComponent implements OnInit {
  createSugarTypeForm!: FormGroup;
  isLoading = false;
  visitTypeListFilerForm!: FormGroup;
  dataSource!: any;
  roleList: any;
  dropdownSettings: any;
  isEditMode: boolean = false;
  pageSize = 1000;
  currentPage = 0;

  constructor(private notificationsService: NotificationsService, private dialog: MatDialog, private formBuilder: FormBuilder, private constantService: ConstantService, private visitTypeService: SugarTypeService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.createSugarTypeForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
    });

    this.LoadData(this.data.element);
  }

  get f() {
    return this.createSugarTypeForm.controls;
  }

  async saveSugarType() {
    this.isLoading = true;
    if (this.createSugarTypeForm.invalid) {
      this.constantService.markFormGroupTouched(this.createSugarTypeForm);
      return;
    }
    let _createSugarTypeForm: any = {};
    _createSugarTypeForm = Object.assign(_createSugarTypeForm, this.createSugarTypeForm.value);

    (await this.visitTypeService.saveSugarType(_createSugarTypeForm)).subscribe(
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
      this.constantService.LoadData(element, this.createSugarTypeForm);
    }
    console.log(this.createSugarTypeForm);
  }


}