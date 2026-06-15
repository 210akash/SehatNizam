import { Component, Inject, OnInit, Optional } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
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
import { ActivatedRoute } from '@angular/router';
import { PatientService } from '../../patient/patient.service';
import { Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, finalize, map, startWith, switchMap } from 'rxjs/operators';
import { EmployeeService } from '../../../hr/employee/employee.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { DoctorService } from '../../doctor/doctor.service';
import { ServiceService } from '../../service/service.service';
import { ReferrerService } from '../../referrer/referrer.service';
import { PrintReceiptAppoinmentComponent } from '../print-receipt-appoinment/print-receipt-appoinment.component';

type Option<T = any> = { id: T; label: string };

@Component({
  selector: 'app-add-appointment',
  templateUrl: './add-appointment.component.html',
  styleUrls: ['./add-appointment.component.css'],
  standalone: false
})
export class AddAppointmentComponent implements OnInit {
  appointmentForm!: FormGroup;
  private initialNavigationState: { element?: any; appointmentStatusId?: number } = {};
  isSubmitting = false;
  successMessage = '';
  errorMessage = '';
  cnicInputMask = createMask('99999-9999999-9');
  phoneNoInputMask = createMask('03999999999');
  emailInputMask = createMask('*[*{0,50}]@*[*{0,50}].*[*{0,5}]');
  cityList: any;
  appointmentTypeList: any[] = [];
  visitTypesList: any;
  paymentModesList: any;
  appointmentStatusList: any;
  minDate = this.toInputDate(new Date());
  priorityLevelList: any;
  paymentStatusList: any;
  departments: any[] = [];
  services: any[] = [];
  patientSearchCtrl = new FormControl<string | any>('');
  filteredPatients$!: Observable<any[]>;
  patientLoading = false;
  private selectedPatientId: number | null = null;
  doctorList: any;
  referrerList: any[] = [];

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
    private activatedRoute: ActivatedRoute,
    private serviceService: ServiceService,
    private referrerService: ReferrerService,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: { element: any, appointmentStatusId: number } | null
  ) { }

  private getRouteState(): { element?: any; appointmentStatusId?: number } {
    const queryStatusId = Number(this.activatedRoute.snapshot.queryParamMap.get('appointmentStatusId'));
    return {
      ...(history.state ?? {}),
      ...(this.router.getCurrentNavigation()?.extras?.state ?? {}),
      ...(this.data ?? {}),
      element: this.data?.element ?? this.router.getCurrentNavigation()?.extras?.state?.['element'] ?? history.state?.element,
      appointmentStatusId: Number.isFinite(queryStatusId) && queryStatusId > 0
        ? queryStatusId
        : (history.state?.appointmentStatusId ?? this.data?.appointmentStatusId)
    };
  }

  ngOnInit(): void {
    this.initialNavigationState = this.getRouteState();
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
      referrerId: [null],
      referrerName: [''],
      referrer: [''],
      patientId: [null],
      doctorName: [''],
      doctor: [''],
      doctorId: [null, Validators.required],
      visitTypeId: [1],
      reason: [''],
      confirmationNotes: [''],
      confirmedDate: [null],
      appointmentStatusId: [this.initialNavigationState.appointmentStatusId ?? 1, Validators.required],
      patient: this.fb.group({
        name: ['', Validators.required],
        phoneNo: ['', Validators.required],
        secondaryPhoneNo: [''],
        address: [''],
        cnic: [''],
        gender: ['male', Validators.required],
        email: ['', Validators.email],
        dateOfBirth: [null],
        age: [0, Validators.required],
        cityId: [1, Validators.required],
        projectId: [0, Validators.required]
      }),
      appointmentPayment: this.fb.array([
        this.createPaymentForm()
      ])
    });
  }

  private get paymentGroup(): FormGroup {
    return (this.appointmentForm.get('appointmentPayment') as FormArray).at(0) as FormGroup;
  }

  private createPaymentForm(): FormGroup {
    return this.fb.group({
      id: [0],
      appointmentId: [0],
      visitFee: [0, Validators.min(0)],
      discount: [0, Validators.min(0)],
      totalPayable: [{ value: 0, disabled: true }],
      paymentModeId: [5, Validators.required],
      serviceId: [null, Validators.required],
      paymentDate: [this.minDate, Validators.required],
      paymentStatusId: [this.initialNavigationState.appointmentStatusId == 1 ? 1 : this.initialNavigationState.appointmentStatusId == 5 ? 3 : 1, Validators.required]
    });
  }

  private setupCalculations(): void {
    const patientGroup = this.appointmentForm.get('patient') as FormGroup;
    patientGroup
      .get('dateOfBirth')
      ?.valueChanges.subscribe((dob) => this.updateAge(dob));

    // const paymentGroup = this.appointmentForm.get('appointmentPayment') as FormGroup;
    // paymentGroup.valueChanges.subscribe(() => this.updateTotalPayable());
    this.paymentGroup.valueChanges.subscribe(() => this.updateTotalPayable());
  }

  private setupPatientAutocomplete(): void {
    this.filteredPatients$ = this.patientSearchCtrl.valueChanges.pipe(
      startWith(''),
      debounceTime(300),
      distinctUntilChanged(),
      switchMap((value: string | any) => {
        const term = typeof value === 'string'
          ? value.trim()
          : (typeof value?.name === 'string' ? value.name.trim() : '');
        if (term.length < 2) {
          return of([]);
        }
        this.patientLoading = true;
        return this.patientService.getPatientByName(term).pipe(
          finalize(() => (this.patientLoading = false))
        );
      })
    );
  }

  private patchEditData(): void {
    const state = this.initialNavigationState.element || this.initialNavigationState.appointmentStatusId
      ? this.initialNavigationState
      : this.getRouteState();
    const element = state.element;
    const appointmentStatusId = state.appointmentStatusId;
    if (!element) {
      if (appointmentStatusId != null) {
        this.appointmentForm.patchValue({ appointmentStatusId });
      }
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
      appointmentStatusId: appointmentStatusId ?? element.appointmentStatusId ?? 1,
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
        serviceId: payment.serviceId ?? 0,
        paymentDate: payment.paymentDate
          ? this.toInputDate(payment.paymentDate)
          : this.minDate
      }
    });

    if (element.patient) {
      this.patientSearchCtrl.setValue(element.patient);
      this.selectedPatientId = element.patientId ?? element.patient?.id ?? null;
    }

    this.updateAge(element.patient?.dateOfBirth);
    this.updateTotalPayable();
  }

  private setupAppointmentStatusWatcher(): void {
    this.appointmentForm.get('appointmentStatusId')?.valueChanges.subscribe((statusId: number) => {
      const paymentGroup = this.appointmentForm.get('appointmentPayment') as FormGroup;

      if (Number(statusId) === 5) {
        // Set paymentStatusId = 3 without affecting other fields
        paymentGroup.patchValue({
          paymentStatusId: 3
        });
      }
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

  public getOPDServiceByDepartment(): void {
    debugger;
    const departmentId = this.appointmentForm.get('departmentId')?.value;
    if (departmentId > 0) {
      const _filterForm = { departmentId: departmentId };
      this.serviceService.getAllServices(_filterForm).subscribe({
        next: (res: any) => {
          // Assign services from response
          this.services = res?.item1 ?? res ?? [];

          // Check if an OPD service exists
          const opdService = this.services.find(
            (s: any) => s.serviceType?.name === 'OPD'
          );

          if (opdService) {
            (this.appointmentForm.get('appointmentPayment') as FormArray)
              .at(0)
              .patchValue({
                serviceId: opdService.id
              });
          }
          else {
            this.notifications.showNotification('No OPD Service Found Against Department', 'snack-bar-danger');
          }
        },
        error: () => {
          // Fallback: keep an empty list; UI will show required validation
          this.services = [];
        }
      });
    }
  }

  getCityList(): void {
    this.cityService.getAllCities({}).subscribe(data => {
      this.cityList = data.item1;

      const lahoreCity = this.cityList.find(
        (city: any) => city.name?.toLowerCase() === 'lahore'
      );

      if (lahoreCity) {
        this.appointmentForm.get('patient.cityId')?.setValue(lahoreCity.id);
      }
    });
  }
  async getAppointmentTypeList(): Promise<void> {
    let _filterForm = {};
    (await this.appointmentTypeService.getAllAppointmentType(_filterForm)).subscribe(data => {
      this.appointmentTypeList = Array.isArray(data?.item1) ? data.item1 : [];
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
    patient ? `${patient.name}${patient.phoneNo ? ' - ' + patient.phoneNo : ''}${patient.cnic ? ' - ' + patient.cnic : ''}` : '';

  onPatientSelected(patient: any): void {
    if (!patient) {
      return;
    }

    this.patientSearchCtrl.setValue(patient.patientMaster, { emitEvent: false });

    this.selectedPatientId = patient.id ?? null;

    const patientGroup = this.appointmentForm.get('patient') as FormGroup;
    patientGroup.patchValue({
      name: patient.patientMaster?.name,
      phoneNo: patient.patientMaster?.phoneNo,
      secondaryPhoneNo: patient.patientMaster?.secondaryPhoneNo,
      address: patient.patientMaster?.address,
      cnic: patient.patientMaster?.cnic,
      gender: patient.patientMaster?.gender || 'male',
      email: patient.patientMaster?.email,
      dateOfBirth: patient.patientMaster?.dateOfBirth ? this.toInputDate(patient.dateOfBirth) : null,
      age: patient.patientMaster?.age,
      cityId: patient.patientMaster?.cityId,
      projectId: patient.projectId ?? 0
    });

    this.appointmentForm.patchValue({
      patientId: patient.id
    });

    // this.updateAge(patient.dateOfBirth);
  }

  onPatientSearchInput(event: Event): void {
    const value = (event.target as HTMLInputElement)?.value?.trim() ?? '';
    if (!value) {
      this.clearPatientSearchFields();
      return;
    }

    if (this.selectedPatientId !== null) {
      this.selectedPatientId = null;
      this.appointmentForm.patchValue({ patientId: null }, { emitEvent: false });
    }

    this.syncNewPatientFromSearch(value);
  }

  private syncNewPatientFromSearch(term: string): void {
    const patientGroup = this.appointmentForm.get('patient') as FormGroup;
    if (this.isPhoneSearchTerm(term)) {
      patientGroup.patchValue({ phoneNo: this.toMaskedPhoneValue(term) }, { emitEvent: false });
      return;
    }

    patientGroup.patchValue({ name: term }, { emitEvent: false });
  }

  private toMaskedPhoneValue(term: string): string {
    let digits = term.replace(/\D/g, '');
    if (!digits) {
      return '';
    }

    if (digits.startsWith('92')) {
      digits = '0' + digits.slice(2);
    }

    if (digits.startsWith('03')) {
      return digits.slice(2);
    }

    return digits;
  }

  private isPhoneSearchTerm(term: string): boolean {
    const trimmed = term.trim();
    if (!trimmed) {
      return false;
    }

    if (/[a-zA-Z\u0600-\u06FF]/.test(trimmed)) {
      return false;
    }

    return /^[\d+\-\s().]+$/.test(trimmed) && /\d/.test(trimmed);
  }

  private clearPatientSearchFields(): void {
    this.patientSearchCtrl.setValue('', { emitEvent: false });
    this.selectedPatientId = null;

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
    if (this.initialNavigationState.appointmentStatusId == 1) {
      this.router.navigate(['/bookappointment']);
      return;
    }
    else {
      this.router.navigate(['/appointment']);
      return;
    }
    // // When opened as a page, navigate back to the appointment list.
    // const canGoBack = window.history.length > 1;
    // if (canGoBack) {
    //   window.history.back();
    // } else {
    //   this.router.navigate(['/appointment']);
    // }
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
    const paymentArray = this.appointmentForm.get('appointmentPayment') as any;
    const firstPayment = paymentArray?.value?.[0];
    const visitFee = Number(firstPayment?.visitFee) || 0;
    const totalPayable = this.calculateTotalPayable();

    // if (statusId === 5 && (visitFee <= 0 || totalPayable <= 0)) {
    //   this.errorMessage = 'For confirmed appointments, Visit Fee and Total Payable must be greater than 0.';
    //   this.notifications.showNotification(this.errorMessage, 'snack-bar-danger');
    //   return;
    // }
    // const paymentGroup = this.appointmentForm.get('appointmentPayment') as FormGroup;
    // const visitFee = Number(paymentGroup.get('visitFee')?.value) || 0;
    // const totalPayable = this.calculateTotalPayable();
    // if (statusId === 5 && (visitFee <= 0 || totalPayable <= 0)) {
    //   this.errorMessage = 'For confirmed appointments, Visit Fee and Total Payable must be greater than 0.';
    //   this.notifications.showNotification(this.errorMessage, 'snack-bar-danger');
    //   return;
    // }

    this.isSubmitting = true;
    const formValue = this.appointmentForm.getRawValue();
    const appointmentDateTime = this.combineDateAndTime(formValue.appointmentDate, formValue.appointmentTime);
    const payload: any = {
      ...formValue,
      patientId: this.selectedPatientId ?? formValue.patientId ?? null,
      appointmentDate: appointmentDateTime,
      patient: {
        ...formValue.patient
      },

      appointmentPayment: [{
        ...formValue.appointmentPayment[0],
        totalPayable: this.calculateTotalPayable()
      }]
    };

    // Keep payment status in sync with appointment status for now
    payload.appointmentPayment.paymentStatusId = payload.appointmentStatusId;

    delete payload.appointmentTime; // not part of backend contract

    this.appointmentService.saveAppointment(payload).subscribe({
      next: (data: { Status: number; Data: string; Message: string }) => {
        if (data.Status === 200) {
          // Navigate back to the list whether opened in dialog or page.
          if (this.initialNavigationState.appointmentStatusId == 1) {
            this.router.navigate(['/bookappointment']);
            this.notifications.showNotification('Booking saved successfully!', 'snack-bar-success');
            return;
          }
          else {
            this.router.navigate(['/appointment']);
    const camelCaseData = this.toCamelCaseObject(data.Data);
            this.printrecreiptAppoinmnetDialog(camelCaseData);
            this.notifications.showNotification('Appointment saved successfully!', 'snack-bar-success');
            return;
          }
        } else if (data.Status === 409) {
          this.errorMessage = data.Message || 'Name Already Exists!';
          this.notifications.showNotification(this.errorMessage, 'snack-bar-danger');
        } else {
          this.errorMessage = data.Message || 'There is some error!';
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
    // const paymentGroup = this.appointmentForm.get('appointmentPayment') as FormGroup;
    // paymentGroup.get('totalPayable')?.setValue(total, { emitEvent: false });
    this.paymentGroup.get('totalPayable')?.setValue(total, { emitEvent: false });
  }

  private calculateTotalPayable(): number {
    // const paymentGroup = this.appointmentForm.get('appointmentPayment') as FormGroup;
    // const fee = Number(paymentGroup.get('visitFee')?.value) || 0;
    // const discount = Number(paymentGroup.get('discount')?.value) || 0;
    const fee = Number(this.paymentGroup.get('visitFee')?.value) || 0;
    const discount = Number(this.paymentGroup.get('discount')?.value) || 0;
    const total = fee - discount;
    return total < 0 ? 0 : Number(total.toFixed(2));
  }

  private combineDateAndTime(date: string | Date, time: string): Date {
    const dateStr = typeof date === 'string' ? date : this.toInputDate(date);
    const [y, m, d] = dateStr.split('-').map(Number);
    const [hours, minutes] = time?.split(':').map((v) => Number(v)) ?? [0, 0];
    return new Date(Date.UTC(y, m - 1, d, hours, minutes, 0, 0));
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
    // if (departmentId == 0 || departmentId == null) {
    //   this.appointmentForm.get('doctorId')?.patchValue(0);
    //   this.appointmentForm.get('doctorName')?.patchValue('');
    //   this.appointmentForm.get('doctor')?.patchValue('');
    //   this.notifications.showNotification('Please Select Department', 'snack-bar-danger');
    // }
    var getDoctorFilter = {
      name: filter,
      departmentId: departmentId
    }
      ; (await this.doctorService.getAllDoctors(getDoctorFilter))
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
    this.appointmentForm.get('departmentId')?.patchValue(selectedValue.departmentId);
    this.appointmentForm.get('doctorId')?.patchValue(selectedValue.id);
    this.appointmentForm.get('doctorName')?.patchValue(this.formatDoctorDisplay(selectedValue));
    this.appointmentForm.get('doctor')?.patchValue(selectedValue);

    const consultationFee = Number(selectedValue?.doctorProfile?.consultationFee ?? 0);
    this.paymentGroup.get('visitFee')?.setValue(consultationFee, { emitEvent: true });
    this.paymentGroup.get('visitFee')?.disable({ emitEvent: false });
    this.getOPDServiceByDepartment();
    // paymentGroup.get('visitFee')?.setValue(consultationFee, { emitEvent: true });
    // paymentGroup.get('visitFee')?.disable({ emitEvent: false });
  }

  getdoctor(itemId: string) {
    return this.doctorList.find((option: { id: string; }) => option.id === itemId);
  }

  onDoctorInputCleared(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    if (!inputValue.trim()) {
      this.appointmentForm.get('departmentId')?.patchValue(0);
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


  // referrer



  async getReferrerList(event: any) {
    var filter = event.currentTarget.value;
    (await this.referrerService.getReferrerByName(filter))
      .subscribe((data: any) => {
        this.referrerList = data;
      });
  }

  onOptionReferrerSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedValue = event.option.value;
    if (!selectedValue) {
      console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');
      return;
    }
    // Get the selected item details from your getaccount method
    const selectedItem = this.getreferrer(selectedValue.id);
    if (!selectedItem) {
      console.error('Selected item not found.');
      return;
    }

    // Patch the values into the form group
    this.appointmentForm.get('referrerId')?.patchValue(selectedValue.id);
    this.appointmentForm.get('referrerName')?.patchValue(this.formatReferrerDisplay(selectedValue));
    this.appointmentForm.get('referrer')?.patchValue(selectedValue);
  }

  private formatReferrerDisplay(referrer: any): string {
    if (!referrer) return '';
    const name = referrer?.name || '';
    const hospital = referrer?.hospital || '';
    return `${name} : ${hospital}`.trim();
  }

  getreferrer(itemId: string) {
    return this.referrerList.find((option: { id: string; }) => option.id === itemId);
  }

  onReferrerInputCleared(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    if (!inputValue.trim()) {
      this.appointmentForm.get('referrerId')?.patchValue(null);
      this.appointmentForm.get('referrerName')?.patchValue('');
      this.appointmentForm.get('referrer')?.patchValue('');
    }
  }

  printrecreiptAppoinmnetDialog(element: any) {
    const dialogRef = this.dialog.open(PrintReceiptAppoinmentComponent, {
      panelClass: 'cstm_width_400',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });
  }

  private toCamelCaseObject(obj: any): any {
    if (Array.isArray(obj)) {
      return obj.map(item => this.toCamelCaseObject(item));
    }

    if (obj !== null && typeof obj === 'object') {
      return Object.keys(obj).reduce((result: any, key) => {
        const camelKey = key.charAt(0).toLowerCase() + key.slice(1);

        result[camelKey] = this.toCamelCaseObject(obj[key]);

        return result;
      }, {});
    }

    return obj;
  }
}
