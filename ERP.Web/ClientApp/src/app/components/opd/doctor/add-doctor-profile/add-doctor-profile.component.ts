import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { DoctorService } from '../doctor.service';

@Component({
  selector: 'app-add-doctor-profile',
  templateUrl: './add-doctor-profile.component.html',
  styleUrls: ['./add-doctor-profile.component.css'],
  standalone: false
})
export class AddDoctorProfileComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;
  isEdit = false;
  doctorName = '';
  doctorDepartment = '';
  doctorDesignation = '';
  doctorCode = '';
  doctorEmail = '';
  doctorPhone = '';
  hospitalAmount = 0;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<AddDoctorProfileComponent>,
    private service: DoctorService,
    private notifications: NotificationsService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    const doctor = this.data?.element ?? {};
    const profile = doctor?.doctorProfile ?? {};
    const profileId = profile?.id ?? 0;

    this.doctorName = `${doctor?.firstName ?? ''} ${doctor?.lastName ?? ''}`.trim();
    this.doctorDepartment = doctor?.department?.name ?? '';
    this.doctorDesignation = doctor?.employeeDesignation?.name ?? '';
    this.doctorCode = doctor?.hrCode || doctor?.code || '';
    this.doctorEmail = doctor?.email ?? '';
    this.doctorPhone = doctor?.phoneNumber ?? '';

    this.isEdit = Number(profileId) > 0;

    this.form = this.fb.group({
      id: [profileId || 0],
      doctorId: [profile?.doctorId || doctor?.id || '', Validators.required],
      pmdcNumber: [profile?.pmdcNumber || ''],
      qualification: [profile?.qualification || ''],
      experienceYears: [profile?.experienceYears ?? 0, Validators.required],
      biography: [profile?.biography || ''],
      specialization: [profile?.specialization || ''],
      consultationFee: [profile?.consultationFee ?? null],
      hospitalPercentage: [profile?.hospitalPercentage ?? null],
      hospitalAmount: [{ value: 0, disabled: true }],
      isAvailableForOPD: [profile?.isAvailableForOPD ?? true],
      isAvailableForIPD: [profile?.isAvailableForIPD ?? true],
      customFieldsJson: [profile?.customFieldsJson || '']
    });

    this.updateHospitalAmount();
    this.form.get('consultationFee')?.valueChanges.subscribe(() => this.updateHospitalAmount());
    this.form.get('hospitalPercentage')?.valueChanges.subscribe(() => this.updateHospitalAmount());
  }

  private updateHospitalAmount(): void {
    const consultationFee = Number(this.form?.get('consultationFee')?.value ?? 0);
    const hospitalPercentage = Number(this.form?.get('hospitalPercentage')?.value ?? 0);

    if (!Number.isFinite(consultationFee) || !Number.isFinite(hospitalPercentage)) {
      this.hospitalAmount = 0;
      return;
    }

    this.hospitalAmount = (consultationFee * hospitalPercentage) / 100;
    this.form?.get('hospitalAmount')?.setValue(this.hospitalAmount, { emitEvent: false });
  }

  save(): void {
    if (this.isLoading) {
      return;
    }

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notifications.showNotification('Please fill all required fields.', 'snack-bar-danger');
      return;
    }

    this.isLoading = true;
    const command = this.form.value;

    this.service.saveDoctorProfile(command).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res.Status === 200) {
          this.notifications.showNotification(res.Data || 'Doctor Profile Saved Successfully!', 'snack-bar-success');
          this.dialogRef.close(true);
        } else if (res.Status === 409) {
          this.notifications.showNotification('Doctor Profile already exists!', 'snack-bar-danger');
        } else {
          this.notifications.showNotification(res.Message || 'Error saving doctor profile!', 'snack-bar-danger');
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
