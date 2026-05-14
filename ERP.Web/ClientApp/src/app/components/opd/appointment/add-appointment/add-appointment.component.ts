import { Component, Inject, OnInit, Optional } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { AppointmentService } from '../appointment.service';
import { DepartmentService } from '../../../department/department.service';
import { createMask } from '@ngneat/input-mask';
import { CityService } from '../../../hr/city/city.service';
import { AppointmentTypeService } from '../../appointment-type/appointment-type.service';
import { VisitTypeService } from '../../visit-type/visit-type.service';
import { PaymentModeService } from '../../../paymentmode/paymentmode.service';
import { PriorityLevelService } from '../../prioritylevel/prioritylevel.service';
import { PrimaryOrderService } from '../../../order/primary-order/order.service';
import { Router } from '@angular/router';
import { PatientService } from '../../patient/patient.service';
import { Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, finalize, map, startWith, switchMap } from 'rxjs/operators';
import { EmployeeService } from '../../../hr/employee/employee.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { DoctorService } from '../../doctor/doctor.service';

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
  appointmentTypeList: any;
  visitTypesList: any;
  paymentModesList: any;
  appointmentStatusList: any;
  minDate = this.toInputDate(new Date());
  priorityLevelList: any;
  paymentStatusList: any;
  departments: any[] = [];
  patientSearchCtrl = new FormControl<string | any>('');
  filteredPatients$!: Observable<any[]>;
  patientLoading = false;
  doctorList: any;
  doctors: Array<{ id: string; name: string; departmentId?: number }> = [
    { id: '408C1D72-07FD-4E9A-A54C-D1AD4112F875', name: 'Dr. Sarah Khan', departmentId: 1 },
    { id: '408C1D72-07FD-4E9A-A54C-D1AD4112F875', name: 'Dr. Ahmed Raza', departmentId: 2 },
    { id: '408C1D72-07FD-4E9A-A54C-D1AD4112F875', name: 'Dr. Maria Aslam', departmentId: 3 },
    { id: '408C1D72-07FD-4E9A-A54C-D1AD4112F875', name: 'Dr. Jason Lee', departmentId: 4 },
    { id: '408C1D72-07FD-4E9A-A54C-D1AD4112F875', name: 'Any Available', departmentId: undefined }
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
    private priorityLevelService: PriorityLevelService,
    private primaryOrderService: PrimaryOrderService,
    private patientService: PatientService,
    private doctorService: DoctorService,
    private router: Router,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: { element: any } | null
  ) { }

  ngOnInit(): void {
    this.loadDepartments();
    this.getCityList();
    this.getAppointmentTypeList();
    this.getVisitTypeList();
    this.getAllAppointmentStatus();
    this.getAllOrderStatus();
    this.getPaymentModesList();
    this.getAllPriorityLevel();
    this.buildForm();
    this.patchEditData();
    this.setupCalculations();
    this.setupPatientAutocomplete();
    this.setupAppointmentStatusWatcher();
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
      doctorName: [''],
      doctor: [''],
      doctorId: [null, Validators.required],
      visitTypeId: [1],
      reason: ['', Validators.required],
      confirmationNotes: [''],
      confirmedDate: [null],
      appointmentStatusId: [1, Validators.required],
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
        cityId: [1, Validators.required],
        projectId: [0, Validators.required]
      }),
      appointmentPayment: this.fb.group({
        id: [0],
        appointmentId: [0],
        visitFee: [0, Validators.min(0)],
        discount: [0, Validators.min(0)],
        totalPayable: [{ value: 0, disabled: true }],
        paymentModeId: [1, Validators.required],
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

  private setupPatientAutocomplete(): void {
    this.filteredPatients$ = this.patientSearchCtrl.valueChanges.pipe(
      startWith(''),
      debounceTime(300),
      distinctUntilChanged(),
      switchMap((value: string | any) => {
        const term = typeof value === 'string' ? value : value || '';
        if (!term || term.length < 2) {
          return of([]);
        }
        this.patientLoading = true;
        return this.patientService.getPatientByName(term).pipe(
          map((data: any) => data?.item1 ?? data ?? []),
          finalize(() => (this.patientLoading = false))
        );
      })
    );
  }

  private patchEditData(): void {
    const element = this.data?.element ?? history.state?.element;
    if (!element) {
      return;
    }

    // Prefer the payment entry that matches this appointmentId; fallback to first or single payment object
    const payment =
      element.appointmentPayments?.find((p: any) => p.appointmentId === element.id) ||
      element.appointmentPayments?.[0] ||
      element.appointmentPayment ||
      {};

    const doctorObj = element.doctor ?? null;
    const doctorDisplay = this.formatDoctorDisplay(doctorObj);

    this.appointmentForm.patchValue({
      ...element,
      appointmentDate: this.toInputDate(element.appointmentDate),
      appointmentTime: this.toInputTime(element.appointmentDate),
      doctor: doctorObj,
      doctorName: doctorDisplay,
      patient: {
        ...element.patient,
        dateOfBirth: element.patient?.dateOfBirth ? this.toInputDate(element.patient.dateOfBirth) : null,
        age: element.patient?.age
      },
      appointmentPayment: {
        ...element.appointmentPayment,
        ...payment,
        appointmentId: payment.appointmentId ?? element.id ?? 0,
        visitFee: payment.visitFee ?? payment.amount ?? element.appointmentPayment?.visitFee ?? 0,
        discount: payment.discount ?? element.appointmentPayment?.discount ?? 0,
        totalPayable: payment.totalPayable ?? element.appointmentPayment?.totalPayable ?? 0,
        paymentModeId: payment.paymentModeId ?? element.appointmentPayment?.paymentModeId ?? 1,
        paymentStatusId: payment.paymentStatusId ?? element.appointmentPayment?.paymentStatusId ?? element.appointmentStatusId ?? 0,
        paymentDate: payment.paymentDate
          ? this.toInputDate(payment.paymentDate)
          : this.minDate
      }
    });

    if (element.patient) {
      this.patientSearchCtrl.setValue(element.patient);
    }

    this.updateAge(element.patient?.dateOfBirth);
    this.updateTotalPayable();
  }

  private setupAppointmentStatusWatcher(): void {
    this.appointmentForm.get('appointmentStatusId')?.valueChanges.subscribe((statusId: number) => {
      if (Number(statusId) === 5) {
        return;
      }
      const paymentGroup = this.appointmentForm.get('appointmentPayment') as FormGroup;
      paymentGroup.reset({
        id: 0,
        appointmentId: this.appointmentForm.get('id')?.value || 0,
        visitFee: 0,
        discount: 0,
        totalPayable: 0,
        paymentModeId: 1,
        paymentDate: this.minDate,
        paymentStatusId: 0
      });
    });
  }

  private loadDepartments(): void {
    this.departmentService.getClinicalDepartment().subscribe({
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

  async getAllPriorityLevel(): Promise<void> {
    let _filterForm = {};
    (await this.priorityLevelService.getAllPriorityLevel(_filterForm)).subscribe(data => {
      this.priorityLevelList = data.item1;
    });
  }

  async getAllOrderStatus(): Promise<void> {
    let _filterForm = {};
    (await this.primaryOrderService.getAllOrderStatus()).subscribe(data => {
      this.paymentStatusList = data;
    });
  }

  async getAllAppointmentStatus(): Promise<void> {
    let _filterForm = {};
    (await this.appointmentService.getAllAppointmentStatus()).subscribe(data => {
      this.appointmentStatusList = data;
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

  displayPatient = (patient: any): string =>
    patient ? `${patient.name}${patient.phoneNo ? ' - ' + patient.phoneNo : ''}` : '';

  onPatientSelected(patient: any): void {
    if (!patient) {
      return;
    }

    this.patientSearchCtrl.setValue(patient, { emitEvent: false });

    const patientGroup = this.appointmentForm.get('patient') as FormGroup;
    patientGroup.patchValue({
      patientId: patient.name,
      name: patient.name,
      phoneNo: patient.phoneNo,
      secondaryPhoneNo: patient.secondaryPhoneNo,
      address: patient.address,
      cnic: patient.cnic,
      gender: patient.gender || 'male',
      email: patient.email,
      dateOfBirth: patient.dateOfBirth ? this.toInputDate(patient.dateOfBirth) : null,
      age: patient.age,
      cityId: patient.cityId,
      projectId: patient.projectId ?? 0
    });

    this.appointmentForm.patchValue({
      patientId: patient.id
    });

    this.updateAge(patient.dateOfBirth);
  }

  onInputCleared(event: Event): void {
    const value = (event.target as HTMLInputElement)?.value?.trim() ?? '';
    if (value.length > 0) {
      return;
    }

    this.patientSearchCtrl.setValue('', { emitEvent: false });

    const patientGroup = this.appointmentForm.get('patient') as FormGroup;
    patientGroup.reset({
      name: '',
      phoneNo: '',
      secondaryPhoneNo: '',
      address: '',
      cnic: '',
      gender: 'male',
      email: '',
      dateOfBirth: null,
      age: null,
      cityId: 1,
      projectId: 0
    });

    this.appointmentForm.patchValue({
      patientId: null
    });
  }

  onCancel(): void {
    if (this.dialog) {
      this.dialog.closeAll();
      this.router.navigate(['/appointment']);
      return;
    }

    // When opened as a page, navigate back to the appointment list.
    const canGoBack = window.history.length > 1;
    if (canGoBack) {
      window.history.back();
    } else {
      this.router.navigate(['/appointment']);
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

    const statusId = Number(this.appointmentForm.get('appointmentStatusId')?.value);
    const paymentGroup = this.appointmentForm.get('appointmentPayment') as FormGroup;
    const visitFee = Number(paymentGroup.get('visitFee')?.value) || 0;
    const totalPayable = this.calculateTotalPayable();
    if (statusId === 5 && (visitFee <= 0 || totalPayable <= 0)) {
      this.errorMessage = 'For confirmed appointments, Visit Fee and Total Payable must be greater than 0.';
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

          // Navigate back to the list whether opened in dialog or page.
          if (this.dialog) {
            this.dialog.closeAll();
          }
          this.router.navigate(['/appointment']);
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

  async getDoctorList(event: any) {
    var filter = event.currentTarget.value;
    var departmentId = this.appointmentForm.get('departmentId')?.value;
    if (departmentId == 0 || departmentId == null) {
      this.appointmentForm.get('doctorId')?.patchValue(0);
      this.appointmentForm.get('doctorName')?.patchValue('');
      this.appointmentForm.get('doctor')?.patchValue('');
      this.notifications.showNotification('Please Select Department', 'snack-bar-danger');
    }
    var getDoctorFilter = {
      name: filter,
      departmentId: departmentId
    }
    ;(await this.doctorService.getAllDoctors(getDoctorFilter))
      .subscribe((data: any) => {
        this.doctorList = data.item1;
      });
  }

  onOptionSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedValue = event.option.value;
    if (!selectedValue) {
      console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');
      return;
    }
    // Get the selected item details from your getaccount method
    const selectedItem = this.getdoctor(selectedValue.id);
    if (!selectedItem) {
      console.error('Selected item not found.');
      return;
    }

    // Patch the values into the form group
    this.appointmentForm.get('doctorId')?.patchValue(selectedValue.id);
    this.appointmentForm.get('doctorName')?.patchValue(this.formatDoctorDisplay(selectedValue));
    this.appointmentForm.get('doctor')?.patchValue(selectedValue);

    const consultationFee = Number(selectedValue?.doctorProfile?.consultationFee ?? 0);
    const paymentGroup = this.appointmentForm.get('appointmentPayment') as FormGroup;
    paymentGroup.get('visitFee')?.setValue(consultationFee, { emitEvent: true });
    paymentGroup.get('visitFee')?.disable({ emitEvent: false });
  }

  getdoctor(itemId: string) {
    return this.doctorList.find((option: { id: string; }) => option.id === itemId);
  }

  onDoctorInputCleared(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    if (!inputValue.trim()) {
      this.appointmentForm.get('doctorId')?.patchValue(0);
      this.appointmentForm.get('doctorName')?.patchValue('');
      this.appointmentForm.get('doctor')?.patchValue('');
      const paymentGroup = this.appointmentForm.get('appointmentPayment') as FormGroup;
      paymentGroup.get('visitFee')?.enable({ emitEvent: false });
      paymentGroup.get('visitFee')?.setValue(0, { emitEvent: true });
    }
  }

  private formatDoctorDisplay(doctor: any): string {
    if (!doctor) return '';
    const code = doctor?.hrCode || '';
    const firstName = doctor?.firstName || '';
    const lastName = doctor?.lastName || '';
    const designation = doctor?.designation || '';
    return `${code} : ${firstName} ${lastName}${designation ? ` (${designation})` : ''}`.trim();
  }
}
