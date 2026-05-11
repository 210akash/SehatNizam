import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { RadiologyTypeService } from '../radiologytype.service';
import { ServiceService } from '../../service/service.service';

@Component({
  selector: 'app-add-radiologytype',
  templateUrl: './add-radiologytype.component.html',
  styleUrls: ['./add-radiologytype.component.css'],
  standalone: false
})
export class AddRadiologyTypeComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;
  isEdit = false;
  services: any[] = [];

  constructor(
    private fb: FormBuilder,
    private dialog: MatDialog,
    private service: RadiologyTypeService,
    private serviceService: ServiceService,
    private notifications: NotificationsService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any; services: any[] }
  ) { }

  ngOnInit(): void {
    this.isEdit = this.data?.element?.id != null;
    this.services = this.data?.services || [];

    this.form = this.fb.group({
      id: [this.isEdit ? this.data.element.id : 0],
      name: [this.isEdit ? this.data.element.name : '', Validators.required],
      serviceId: [this.isEdit ? this.data.element.serviceId : null, Validators.required]
    });
        this.getservicesList();
  }

 getservicesList() {
    let _CategoryFilter: any = {DepartmentId : 28};
    this.serviceService.getAllServices(_CategoryFilter).subscribe((data: any) => {
      this.services = data.item1;
    });
  }


  save(): void {
    if (this.form.invalid) return;

    this.isLoading = true;
    const command = this.form.value;

    this.service.saveRadiologyType(command).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res.Status === 200) {
          this.notifications.showNotification(res.Data || 'Radiology Type Saved Successfully!', 'snack-bar-success');
          this.dialog.closeAll();
        } else if (res.Status === 409) {
          this.notifications.showNotification('Radiology Type with this name already exists!', 'snack-bar-danger');
        } else {
          this.notifications.showNotification(res.Message || 'Error saving radiology type!', 'snack-bar-danger');
        }
      },
      error: (error: any) => {
        this.isLoading = false;
        const message = error?.error?.Message || 'An error occurred';
        this.notifications.showNotification(message, 'snack-bar-danger');
      }
    });
  }
}
