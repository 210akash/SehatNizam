import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { SurgicalOrderService } from '../surgical-order.service';
import { ServiceService } from '../../service/service.service';
import { DoctorService } from '../../doctor/doctor.service';
import { SurgicalOrderListComponent } from '../surgical-order-list/surgical-order-list.component';

@Component({
  selector: 'app-manage-surgical-order',
  templateUrl: './manage-surgical-order.component.html',
  styleUrls: ['./manage-surgical-order.component.css'],
  standalone: false
})
export class ManageSurgicalOrderComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;
  surgicalServices: any[] = [];
  doctors: any[] = [];
  reloadToken = 0;

  @ViewChild(SurgicalOrderListComponent) orderList?: SurgicalOrderListComponent;

  constructor(
    private fb: FormBuilder,
    private notifications: NotificationsService,
    private surgicalOrderService: SurgicalOrderService,
    private serviceService: ServiceService,
    private doctorService: DoctorService,
    private dialogRef: MatDialogRef<ManageSurgicalOrderComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    const now = new Date();
    const scheduledTime = `${String(now.getHours()).padStart(2, '0')}:${String(now.getMinutes()).padStart(2, '0')}`;

    this.form = this.fb.group({
      id: [0],
      appointmentId: [this.data.element.id, Validators.required],
      serviceId: ['', Validators.required],
      surgeonId: [this.data.element.doctorId || '', Validators.required],
      scheduledDate: [now, Validators.required],
      scheduledTime: [scheduledTime, Validators.required],
      statusId: [1],
      clinicalNotes: ['']
    });

    this.loadLookups();
  }

  loadLookups(): void {
    this.serviceService.getAllServices({ isSurgical: true, pagingData: { currentPage: 0, take: 500 } }).subscribe({
      next: (res: any) => this.surgicalServices = res?.item1 ?? [],
      error: () => this.surgicalServices = []
    });

    this.doctorService.getAllDoctors({ pagingData: { currentPage: 0, take: 500 } }).then(obs => {
      obs.subscribe({
        next: (res: any) => this.doctors = res?.item1 ?? [],
        error: () => this.doctors = []
      });
    });
  }

  getPatientName(): string {
    return this.data.element?.patient?.patientMaster?.name || '-';
  }

  getPatientPhone(): string {
    return this.data.element?.patient?.patientMaster?.phoneNo || '-';
  }

  getDoctorName(): string {
    const d = this.data.element?.doctor;
    return d ? `${d.firstName || ''} ${d.lastName || ''}`.trim() : '-';
  }

  getDepartmentName(): string {
    return this.data.element?.department?.name || '-';
  }

  private buildScheduledDateTime(): Date | null {
    const dateValue = this.form.value.scheduledDate;
    const timeValue = this.form.value.scheduledTime;
    if (!dateValue || !timeValue) return null;

    const date = new Date(dateValue);
    const [hours, minutes] = String(timeValue).split(':').map((v: string) => Number(v));
    if (Number.isNaN(hours) || Number.isNaN(minutes)) return null;

    date.setHours(hours, minutes, 0, 0);
    return date;
  }

  save(): void {
    if (this.form.invalid) {
      this.notifications.showNotification('Please fill required fields', 'snack-bar-danger');
      return;
    }

    const scheduledDateTime = this.buildScheduledDateTime();
    if (!scheduledDateTime) {
      this.notifications.showNotification('Please select valid date and time', 'snack-bar-danger');
      return;
    }

    this.isLoading = true;
    const payload = {
      ...this.form.value,
      scheduledDateTime: scheduledDateTime.toISOString()
    };

    delete payload.scheduledDate;
    delete payload.scheduledTime;

    this.surgicalOrderService.saveSurgicalOrder(payload).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res?.Status === 200) {
          this.notifications.showNotification('Surgical order saved', 'snack-bar-success');
          this.form.patchValue({ id: 0, clinicalNotes: '', statusId: 1 });
          this.reloadToken++;
          this.orderList?.bindData();
        } else {
          this.notifications.showNotification(res?.Message || 'Save failed', 'snack-bar-danger');
        }
      },
      error: () => {
        this.isLoading = false;
        this.notifications.showNotification('Save failed', 'snack-bar-danger');
      }
    });
  }

  close(): void {
    this.dialogRef.close(true);
  }
}
