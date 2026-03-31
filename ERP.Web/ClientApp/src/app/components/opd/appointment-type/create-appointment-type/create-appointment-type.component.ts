import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { AppointmentTypeService } from '../appointment-type.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-create-appointment-type',
  templateUrl: './create-appointment-type.component.html',
  styleUrls: ['./create-appointment-type.component.css'],standalone: false
})

export class CreateAppointmentTypeComponent implements OnInit {
  createAppointmentTypeForm!: FormGroup;
  isLoading = false;
  appointmentTypeListFilerForm!: FormGroup;
  dataSource!: any;
  roleList: any;
  dropdownSettings: any;
  isEditMode: boolean = false;
  pageSize = 1000;
  currentPage = 0;

  constructor(private notificationsService: NotificationsService, private dialog: MatDialog, private formBuilder: FormBuilder, private constantService: ConstantService, private appointmentTypeService: AppointmentTypeService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.createAppointmentTypeForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
    });

    this.LoadData(this.data.element);
  }

  get f() {
    return this.createAppointmentTypeForm.controls;
  }

  async saveAppointmentType() {
    this.isLoading = true;
    if (this.createAppointmentTypeForm.invalid) {
      this.constantService.markFormGroupTouched(this.createAppointmentTypeForm);
      return;
    }
    let _createAppointmentTypeForm: any = {};
    _createAppointmentTypeForm = Object.assign(_createAppointmentTypeForm, this.createAppointmentTypeForm.value);

    (await this.appointmentTypeService.saveAppointmentType(_createAppointmentTypeForm)).subscribe(
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
      this.constantService.LoadData(element, this.createAppointmentTypeForm);
    }
    console.log(this.createAppointmentTypeForm);
  }


}