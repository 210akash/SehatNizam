import { Component, Inject, OnInit, Optional } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { AppointmentService } from '../appointment.service';
import { DepartmentService } from '../../../department/department.service';
import { createMask } from '@ngneat/input-mask';
import { CityService } from '../../../hr/city/city.service';
import { AppointmentTypeService } from '../../appointment-type/appointment-type.service';
import { VisitTypeService } from '../../visit-type/visit-type.service';
import { PaymentModeService } from '../../../paymentmode/paymentmode.service';

type Option<T = any> = { id: T; label: string };

@Component({
  selector: 'app-add-appointment',
  templateUrl: './add-appointment.component.html',
  styleUrls: ['./add-appointment.component.css'],
  standalone: false
})
export class AddAppointmentComponent implements OnInit {
  appointmentForm!: FormGroup;
  isSubmitting = false;
  successMessage = '';
  errorMessage = '';
  cnicInputMask = createMask('99999-9999999-9');
  phoneNoInputMask = createMask('0399-9999999');
  emailInputMask = createMask('*[*{0,50}]@*[*{0,50}].*[*{0,5}]');
  cityList: any;
  appointmentTypeList : any;
  visitTypesList : any;
  paymentModesList : any;
  minDate = this.toInputDate(new Date());

  departments: any[] = [];
  doctors: Array<{ id: string; name: string; departmentId?: number }> = [
    { id: '7c9e6679-7425-40de-944b-e07fc1f90ae7', name: 'Dr. Sarah Khan', departmentId: 1 },
    { id: 'b46e6e71-bafd-4bb5-9a7b-27b9719a5e2d', name: 'Dr. Ahmed Raza', departmentId: 2 },
    { id: '1f4a9e3a-2f56-41fc-a6fb-20d8f3a1c9a2', name: 'Dr. Maria Aslam', departmentId: 3 },
    { id: 'c7f2b3b3-9dd5-4de6-8b45-6f5c7e9a5f8e', name: 'Dr. Jason Lee', departmentId: 4 },
    { id: '14fae5f8-cf4f-4af1-92d2-1f3d5cbd8f99', name: 'Any Available', departmentId: undefined }
  ];

  priorityLevels: Option<number>[] = [
    { id: 1, label: 'Normal' },
    { id: 2, label: 'Urgent' },
    { id: 3, label: 'Emergency' },
    { id: 4, label: 'Critical' }
  ];

  appointmentStatuses: Option<number>[] = [
    { id: 0, label: 'Pending (Only Register)' },
    { id: 1, label: 'Confirmed (Payment Received)' }
  ];

  constructor(
    private fb: FormBuilder,
    private dialog: MatDialog,
    private notifications: NotificationsService,
    private appointmentService: AppointmentService,
    private departmentService: DepartmentService,
    private cityService: CityService,
    private appointmentTypeService: AppointmentTypeService,
    private visitTypeService: VisitTypeService,
    private paymentModeService: PaymentModeService,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: { element: any } | null
  ) {}

  ngOnInit(): void {
    this.buildForm();
    this.loadDepartments();
    this.getCityList();
    this.getAppointmentTypeList();
    this.getVisitTypeList();
    this.patchEditData();
    this.setupCalculations();
  }

  private buildForm(): void {
    this.appointmentForm = this.fb.group({
      id: [0],
      appointmentDate: [this.minDate, Validators.required],
      appointmentTime: [this.toInputTime(new Date()), Validators.required],
      tokenNumber: [''],
      appointmentTypeId: [1],
      priorityLevelId: [1, Validators.required],
      departmentId: [null, Validators.required],
      patientId: [null],
      doctorId: [null, Validators.required],
      visitTypeId: [this.visitTypesList[0].id],
      reason: ['', Validators.required],
      confirmationNotes: [''],
      confirmedDate: [null],
      appointmentStatusId: [0, Validators.required],
      patient: this.fb.group({
        name: ['', Validators.required],
        phoneNo: ['', Validators.required],
        secondaryPhoneNo: [''],
        address: [''],
        cnic: [''],
        gender: ['male', Validators.required],
        email: ['', Validators.email],
        dateOfBirth: [null, Validators.required],
        age: [{ value: null, disabled: true }],
        cityId: [this.cityList[0]?.id ?? null, Validators.required],
        projectId: [0, Validators.required]
      }),
      appointmentPayment: this.fb.group({
        id: [0],
        appointmentId: [0],
        visitFee: [0, Validators.min(0)],
        discount: [0, Validators.min(0)],
        totalPayable: [{ value: 0, disabled: true }],
        paymentModeId: [this.paymentModesList[0].id, Validators.required],
        paymentDate: [this.minDate, Validators.required],
        paymentStatusId: [0, Validators.required]
      })
    });
  }

  private setupCalculations(): void {
    const patientGroup = this.appointmentForm.get('patient') as FormGroup;
    patientGroup
      .get('dateOfBirth')
      ?.valueChanges.subscribe((dob) => this.updateAge(dob));

    const paymentGroup = this.appointmentForm.get('appointmentPayment') as FormGroup;
    paymentGroup.valueChanges.subscribe(() => this.updateTotalPayable());
  }

  private patchEditData(): void {
    if (!this.data?.element) {
      return;
    }

    const element = this.data.element;
    this.appointmentForm.patchValue({
      ...element,
      appointmentDate: this.toInputDate(element.appointmentDate),
      appointmentTime: this.toInputTime(element.appointmentDate),
      patient: {
        ...element.patient,
        dateOfBirth: element.patient?.dateOfBirth ? this.toInputDate(element.patient.dateOfBirth) : null,
        age: element.patient?.age
      },
      appointmentPayment: {
        ...element.appointmentPayment,
        paymentDate: element.appointmentPayment?.paymentDate
          ? this.toInputDate(element.appointmentPayment.paymentDate)
          : this.minDate
      }
    });

    this.updateAge(element.patient?.dateOfBirth);
    this.updateTotalPayable();
  }

  private loadDepartments(): void {
    this.departmentService.getAllDepartments({}).subscribe({
      next: (res: any) => {
        this.departments = res?.item1 ?? res ?? [];
      },
      error: () => {
        // Fallback: keep an empty list; UI will show required validation
        this.departments = [];
      }
    });
  }
  
  getCityList(): void {
    let _filterForm = {};
    this.cityService.getAllCities(_filterForm).subscribe(data => {
      this.cityList = data.item1;
    });
  }

    async getAppointmentTypeList(): Promise<void> {
    let _filterForm = {};
    (await this.appointmentTypeService.getAllAppointmentType(_filterForm)).subscribe(data => {
      this.appointmentTypeList = data.item1;
    });
  }

   async getVisitTypeList(): Promise<void> {
    let _filterForm = {};
    (await this.visitTypeService.getAllVisitType(_filterForm)).subscribe(data => {
      this.visitTypesList = data.item1;
    });
  }

  async getPaymentModesList(): Promise<void> {
    let _filterForm = {};
    (await this.paymentModeService.getAllPaymentModes(_filterForm)).subscribe(data => {
      this.paymentModesList = data.item1;
    });
  }

  onCancel(): void {
    if (this.dialog) {
      this.dialog.closeAll();
    } else {
      window.history.back();
    }
  }

  onSubmit(): void {
    this.successMessage = '';
    this.errorMessage = '';

    if (this.appointmentForm.invalid) {
      this.appointmentForm.markAllAsTouched();
      this.errorMessage = 'Please fill all required fields.';
      this.notifications.showNotification(this.errorMessage, 'snack-bar-danger');
      return;
    }

    this.isSubmitting = true;
    const formValue = this.appointmentForm.getRawValue();

    const appointmentDateTime = this.combineDateAndTime(formValue.appointmentDate, formValue.appointmentTime);
    const payload: any = {
      ...formValue,
      appointmentDate: appointmentDateTime,
      patient: {
        ...formValue.patient,
        age: this.calculateAge(formValue.patient.dateOfBirth)
      },
      appointmentPayment: {
        ...formValue.appointmentPayment,
        totalPayable: this.calculateTotalPayable()
      }
    };

    // Keep payment status in sync with appointment status for now
    payload.appointmentPayment.paymentStatusId = payload.appointmentStatusId;

    delete payload.appointmentTime; // not part of backend contract

    this.appointmentService.saveAppointment(payload).subscribe({
      next: (data: { Status: number; Data: string; Message: string }) => {
        if (data.Status === 200) {
          this.successMessage = data.Data || 'Appointment Saved!';
          this.notifications.showNotification(this.successMessage, 'snack-bar-success');
          setTimeout(() => this.onCancel(), 800);
        } else if (data.Status === 409) {
          this.errorMessage = data.Data || 'Name Already Exists!';
          this.notifications.showNotification(this.errorMessage, 'snack-bar-danger');
        } else {
          this.errorMessage = data.Message || data.Data || 'There is some error!';
          this.notifications.showNotification(this.errorMessage, 'snack-bar-danger');
        }
        this.isSubmitting = false;
      },
      error: (error: any) => {
        const message = error?.error?.Message || error?.error?.Data || error.statusText || 'An unexpected error occurred.';
        this.errorMessage = message;
        this.notifications.showNotification(message, 'snack-bar-danger');
        this.isSubmitting = false;
      }
    });
  }

  // Helpers
  private updateAge(dob: string | Date | null): void {
    const age = this.calculateAge(dob);
    const patientGroup = this.appointmentForm.get('patient') as FormGroup;
    patientGroup.get('age')?.setValue(age, { emitEvent: false });
  }

  private calculateAge(dob: string | Date | null): number | null {
    if (!dob) return null;
    const birthDate = new Date(dob);
    const diff = Date.now() - birthDate.getTime();
    const ageDate = new Date(diff);
    return Math.abs(ageDate.getUTCFullYear() - 1970);
  }

  private updateTotalPayable(): void {
    const total = this.calculateTotalPayable();
    const paymentGroup = this.appointmentForm.get('appointmentPayment') as FormGroup;
    paymentGroup.get('totalPayable')?.setValue(total, { emitEvent: false });
  }

  private calculateTotalPayable(): number {
    const paymentGroup = this.appointmentForm.get('appointmentPayment') as FormGroup;
    const fee = Number(paymentGroup.get('visitFee')?.value) || 0;
    const discount = Number(paymentGroup.get('discount')?.value) || 0;
    const total = fee - discount;
    return total < 0 ? 0 : Number(total.toFixed(2));
  }

  filterDoctors(): Array<{ id: string; name: string; departmentId?: number }> {
    const departmentId = this.appointmentForm.get('departmentId')?.value;
    if (!departmentId) return this.doctors;
    return this.doctors.filter((d) => !d.departmentId || d.departmentId === departmentId);
  }

  private combineDateAndTime(date: string | Date, time: string): Date {
    const baseDate = new Date(date);
    const [hours, minutes] = time?.split(':').map((v) => Number(v)) ?? [0, 0];
    baseDate.setHours(hours, minutes, 0, 0);
    return baseDate;
  }

  private toInputDate(date: Date | string): string {
    const d = new Date(date);
    const month = `${d.getMonth() + 1}`.padStart(2, '0');
    const day = `${d.getDate()}`.padStart(2, '0');
    return `${d.getFullYear()}-${month}-${day}`;
  }

  private toInputTime(date: Date | string): string {
    const d = new Date(date);
    const hours = `${d.getHours()}`.padStart(2, '0');
    const minutes = `${d.getMinutes()}`.padStart(2, '0');
    return `${hours}:${minutes}`;
  }
}
