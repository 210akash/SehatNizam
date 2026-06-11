import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { ServiceService } from '../service.service';
import { DepartmentService } from '../../../department/department.service';
import { ServiceTypeService } from '../../service-type/service-type.service';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-add-service',
  templateUrl: './add-service.component.html',
  styleUrls: ['./add-service.component.css'],
  standalone: false
})
export class AddServiceComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;
  isEdit = false;
  departments: any[] = [];
  serviceType : any[] = [];

  constructor(
    private fb: FormBuilder,
    private dialog: MatDialog,
    private service: ServiceService,
    private notifications: NotificationsService,
    private departmentService: DepartmentService,
    private serviceTypeService: ServiceTypeService,
    private constantService: ConstantService, 
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      id: [0],
      code: ['', Validators.required],
      name: ['', Validators.required],
      description: ['', Validators.required],
      basePrice: [ 0, [Validators.required, Validators.min(0)]],
      departmentId: ['', Validators.required],
      serviceTypeId: ['', Validators.required]
    });
    this.loadServiceType();
    this.loadDepartments();
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEdit = true;
      this.constantService.LoadData(element, this.form);
    }
    else   
     this.getServiceCode();
  }

  getServiceCode() {
    this.service.getServiceCode().subscribe((data: any) => {
      this.form.get('code')?.patchValue(data.code);
      console.log(data.code);
      console.log(this.form.get('code')?.value);
    });
  }

  loadServiceType(): void {
    this.serviceTypeService.getAllServiceTypes({}).subscribe({
      next: (res: any) => {
        this.serviceType = res?.item1 ?? res ?? [];
      },
      error: () => {
        this.serviceType = [];
      }
    });
  }

  loadDepartments(): void {
    this.departmentService.getClinicalDepartment().subscribe({
      next: (res: any) => {
        this.departments = res?.item1 ?? res ?? [];
      },
      error: () => {
        this.departments = [];
      }
    });
  }

  save(): void {
    if (this.form.invalid) return;

    this.isLoading = true;
    const command = this.form.value;

    this.service.saveService(command).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res.Status === 200) {
          this.notifications.showNotification(res.Data || 'Service Saved Successfully!', 'snack-bar-success');
          this.dialog.closeAll();
        } else if (res.Status === 409) {
          this.notifications.showNotification('Service with this code already exists!', 'snack-bar-danger');
        } else {
          this.notifications.showNotification(res.Message || 'Error saving service!', 'snack-bar-danger');
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
