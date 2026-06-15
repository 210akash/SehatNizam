import { Component, Inject, OnInit, Optional } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { DepartmentService } from '../../../department/department.service';
import { createMask } from '@ngneat/input-mask';
import { CityService } from '../../../hr/city/city.service';
import { PaymentModeService } from '../../../paymentmode/paymentmode.service';
import { PrimaryOrderService } from '../../../order/primary-order/order.service';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, finalize, map, startWith, switchMap } from 'rxjs/operators';
import { EmployeeService } from '../../../hr/employee/employee.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { AdmissionService } from '../admission.service';
import { AppointmentTypeService } from '../../../opd/appointment-type/appointment-type.service';
import { VisitTypeService } from '../../../opd/visit-type/visit-type.service';
import { PriorityLevelService } from '../../../opd/prioritylevel/prioritylevel.service';
import { PatientService } from '../../../opd/patient/patient.service';
import { DoctorService } from '../../../opd/doctor/doctor.service';
import { ServiceService } from '../../../opd/service/service.service';
import { ReferrerService } from '../../../opd/referrer/referrer.service';
import { AppointmentService } from '../../../opd/appointment/appointment.service';
import { AdmissionPackageService } from '../../admission-package/admission-package.service';

type Option<T = any> = { id: T; label: string };

@Component({
  selector: 'app-add-admission',
  templateUrl: './add-admission.component.html',
  styleUrls: ['./add-admission.component.css'],
  standalone: false
})
export class AddAdmissionComponent implements OnInit {
  admissionForm!: FormGroup;
  private initialNavigationState: { element?: any; appointmentStatusId?: number } = {};
  isSubmitting = false;
  successMessage = '';
  errorMessage = '';
  cnicInputMask = createMask('99999-9999999-9');
  phoneNoInputMask = createMask('03999999999');
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
  services: any[] = [];
  patientSearchCtrl = new FormControl<string | any>('');
  filteredPatients$!: Observable<any[]>;
  patientLoading = false;
  doctorList: any;
  referrerList: any[] = [];
  packageList: any[] = [];
  selectedPackageDetails: any[] = [];
  packageTotal: number = 0;
  selectedServiceId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private dialog: MatDialog,
    private notifications: NotificationsService,
    private admissionService: AdmissionService,
    private departmentService: DepartmentService,
    private cityService: CityService,
    private appointmentTypeService: AppointmentTypeService,
    private appointmentService: AppointmentService,
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
    private admissionPackageService: AdmissionPackageService,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: { element: any} | null
  ) { }

  ngOnInit(): void {
    this.loadDepartments();
    this.getCityList();
    this.getPackageList();
    this.getAppointmentTypeList();
    this.getVisitTypeList();
    this.getAllAppointmentStatus();
    this.getAllOrderStatus();
    this.getPaymentModesList();
    this.getAllPriorityLevel();
    this.buildForm();
    //this.patchEditData();
    this.setupCalculations();
    this.setupPatientAutocomplete();
    this.setupAppointmentStatusWatcher();
  }

  private buildForm(): void {
    this.admissionForm = this.fb.group({
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
      visitTypeId: [3],
      reason: ['', Validators.required],
      confirmationNotes: [''],
      admissionDiagnosis: ['', Validators.required],
      confirmedDate: [null],
      appointmentStatusId: [30, Validators.required],
      totalPackageAmount: [0, Validators.required],
      admissionPackageMasterId : [null,Validators.required],
      paymentModeId: [5, Validators.required],
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
      ])
    });
  }

  private get paymentGroup(): FormGroup {
    return (this.admissionForm.get('appointmentPayment') as FormArray).at(0) as FormGroup;
  }

  private createPaymentForm(): FormGroup {
    return this.fb.group({
      id: [0],
      appointmentId: [0],
      visitFee: [0, Validators.min(0)],
      discount: [0, Validators.min(0)],
      totalPayable: [0],
      paymentModeId: [5, Validators.required],
      serviceId: [null, Validators.required],
      paymentDate: [this.minDate, Validators.required],
      paymentStatusId: [this.initialNavigationState.appointmentStatusId == 1 ? 1 : this.initialNavigationState.appointmentStatusId == 5 ? 3 : 1, Validators.required]
    });
  }

  private setupCalculations(): void {
    const patientGroup = this.admissionForm.get('patient') as FormGroup;
    patientGroup
      .get('dateOfBirth')
      ?.valueChanges.subscribe((dob) => this.updateAge(dob));

    this.admissionForm.get('appointmentPayment')?.valueChanges.subscribe(() => {
      this.updateTotalPayable();
    });
  }

  get appointmentPayments(): FormArray {
    return this.admissionForm.get('appointmentPayment') as FormArray;
  }

  addServiceToPayment(): void {
    if (!this.selectedServiceId) {
      this.notifications.showNotification('Please select a service first', 'snack-bar-danger');
      return;
    }

    const selectedService = this.services.find(s => s.id === this.selectedServiceId);
    if (!selectedService) return;

    const paymentExists = this.appointmentPayments.controls.some(
      (ctrl: any) => ctrl.get('serviceId')?.value === this.selectedServiceId
    );

    if (paymentExists) {
      this.notifications.showNotification('Service already added', 'snack-bar-danger');
      return;
    }

    this.appointmentPayments.push(this.fb.group({
      id: [0],
      appointmentId: [0],
      visitFee: [selectedService.basePrice || 0],
      discount: [0, Validators.min(0)],
      totalPayable: [0],
      paymentModeId: [5, Validators.required],
      serviceId: [this.selectedServiceId, Validators.required],
      paymentDate: [this.minDate, Validators.required],
      paymentStatusId: [1, Validators.required]
    }));

    this.selectedServiceId = null;
    this.updateTotalPayable();
  }

  removeService(index: number): void {
    if (this.appointmentPayments.length > 1 || index !== 0) {
      this.appointmentPayments.removeAt(index);
      this.updateTotalPayable();
    }
  }

  getServiceName(serviceId: number): string {
    const service = this.services.find(s => s.id === serviceId);
    return service ? service.name : '';
  }

  calculateRowAmount(row: any): number {
    const fee = Number(row.get('visitFee')?.value) || 0;
    const discount = Number(row.get('discount')?.value) || 0;
    return fee - discount;
  }

  hasServicesAdded(): boolean {
    return this.appointmentPayments.controls.some(
      (ctrl: any) => ctrl.get('serviceId')?.value
    );
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
    // const state = this.initialNavigationState.element || this.initialNavigationState.appointmentStatusId
    //   ? this.initialNavigationState
    //   : this.getRouteState();
    // const element = state.element;
    // const appointmentStatusId = state.appointmentStatusId;
    // if (!element) {
    //   if (appointmentStatusId != null) {
    //     this.admissionForm.patchValue({ appointmentStatusId });
    //   }
    //   return;
    // }
    var element  = this.data?.element;
    // Prefer the payment entry that matches this appointmentId; fallback to first or single payment object
    const payment =
      element.appointmentPayments?.find((p: any) => p.appointmentId === element.id) ||
      element.appointmentPayments?.[0] ||
      element.appointmentPayment ||
      {};

    const doctorObj = element.doctor ?? null;
    const doctorDisplay = this.formatDoctorDisplay(doctorObj);

    this.admissionForm.patchValue({
      ...element,
      appointmentStatusId: element.appointmentStatusId ?? 1,
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
    }

    this.updateAge(element.patient?.dateOfBirth);
    this.updateTotalPayable();
  }

  private setupAppointmentStatusWatcher(): void {
    this.admissionForm.get('appointmentStatusId')?.valueChanges.subscribe((statusId: number) => {
      if (Number(statusId) === 5) {
        // Set paymentStatusId = 3 for all payments
        this.appointmentPayments.controls.forEach((payment: any) => {
          payment.get('paymentStatusId')?.patchValue(3);
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
public getIPDService(): void {
  const departmentId = this.admissionForm.get('departmentId')?.value;
  const _filterForm = { departmentId: departmentId };

  this.serviceService.getAllServices(_filterForm).subscribe({
    next: (res: any) => {
      // Assign services from response
      const allServices = res?.item1 ?? res ?? [];

      // Get all IPD services
      this.services = allServices.filter(
        (s: any) => s.serviceType?.name === 'IPD'
      );

      // Optional: if you want to handle the case when no IPD services exist
      if (this.services.length === 0) {
        this.notifications.showNotification('No IPD Service Found Against Department', 'snack-bar-danger');
      }
    },
    error: () => {
      // Fallback: keep an empty list; UI will show required validation
      this.services = [];
    }
  });
}

  getCityList(): void {
    let _filterForm = {};
    this.cityService.getAllCities(_filterForm).subscribe(data => {
      this.cityList = data.item1;
    });
  }

getPackageList(): void {
    let _filterForm = {};
    this.admissionPackageService.getAllAdmissionPackages(_filterForm).subscribe(data => {
      this.packageList = data.item1;
    });
  }

  onPackageSelected(packageId: number): void {
    if (!packageId) {
      this.selectedPackageDetails = [];
      this.packageTotal = 0;
      this.admissionForm.get('totalPackageAmount')?.setValue(0);
      return;
    }
    const selectedPackage = this.packageList.find((p: any) => p.id === packageId);
    if (selectedPackage && selectedPackage.admissionPackageDetail) {
      this.selectedPackageDetails = selectedPackage.admissionPackageDetail;
      this.packageTotal = this.selectedPackageDetails.reduce(
        (sum: number, item: any) => sum + (item.service?.basePrice || 0),
        0
      );
    } else {
      this.selectedPackageDetails = [];
      this.packageTotal = 0;
    }
    this.admissionForm.get('totalPackageAmount')?.setValue(this.packageTotal);
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
    patient ? `${patient.name}${patient.phoneNo ? ' - ' + patient.phoneNo : ''}${patient.cnic ? ' - ' + patient.cnic : ''}` : '';

  onPatientSelected(patient: any): void {
    if (!patient) {
      return;
    }

    this.patientSearchCtrl.setValue(patient.patientMaster, { emitEvent: false });

    const patientGroup = this.admissionForm.get('patient') as FormGroup;
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

    this.admissionForm.patchValue({
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

    const patientGroup = this.admissionForm.get('patient') as FormGroup;
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

    this.admissionForm.patchValue({
      patientId: null
    });
  }

  onCancel(): void {
      this.router.navigate(['/admission']);
      return;
  }

onSubmit(): void {
    this.successMessage = '';
    this.errorMessage = '';

    if (this.admissionForm.invalid) {
      this.admissionForm.markAllAsTouched();
      this.errorMessage = 'Please fill all required fields.';
      this.notifications.showNotification(this.errorMessage, 'snack-bar-danger');
      return;
    }

    if (!this.hasServicesAdded()) {
      this.errorMessage = 'Please add at least one service.';
      this.notifications.showNotification(this.errorMessage, 'snack-bar-danger');
      return;
    }

    const statusId = Number(this.admissionForm.get('appointmentStatusId')?.value);
    const totalPayable = this.calculateTotalPayable();

    // if (statusId === 5 && totalPayable <= 0) {
    //   this.errorMessage = 'For confirmed appointments, Total Payable must be greater than 0.';
    //   this.notifications.showNotification(this.errorMessage, 'snack-bar-danger');
    //   return;
    // }

    this.isSubmitting = true;
    const formValue = this.admissionForm.getRawValue();
    const appointmentDateTime = this.combineDateAndTime(formValue.appointmentDate, formValue.appointmentTime);

    // Filter payment rows to only include those with a valid serviceId
    const payments = formValue.appointmentPayment.filter((p: any) => p.serviceId);

    const payload: any = {
      ...formValue,
      appointmentDate: appointmentDateTime,
      patient: {
        ...formValue.patient
      },
      appointmentPayments: payments.map((p: any) => ({
        ...p,
        totalPayable: totalPayable,
        paymentStatusId: formValue.appointmentStatusId
      }))
    };

    delete payload.appointmentTime; // not part of backend contract

    this.admissionService.saveAdmission(payload).subscribe({
      next: (data: { Status: number; Data: string; Message: string }) => {
        if (data.Status === 200) {
          this.successMessage = data.Data || 'Admission Saved!';
          this.notifications.showNotification(this.successMessage, 'snack-bar-success');

          // Navigate back to the list whether opened in dialog or page.
          if (this.initialNavigationState.appointmentStatusId == 1) {
            this.router.navigate(['/bookappointment']);
            return;
          }
          else {
            this.router.navigate(['/admission']);
            return;
          }
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
    const patientGroup = this.admissionForm.get('patient') as FormGroup;
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
    this.appointmentPayments.controls.forEach((payment: any) => {
      payment.get('totalPayable')?.setValue(total, { emitEvent: false });
    });
  }

  private calculateTotalPayable(): number {
    const payments = this.appointmentPayments.controls;
    let total = 0;
    payments.forEach((payment: any) => {
      const fee = Number(payment.get('visitFee')?.value) || 0;
      const discount = Number(payment.get('discount')?.value) || 0;
      const rowAmount = fee - discount;
      total += rowAmount > 0 ? rowAmount : 0;
    });
    return Number(total.toFixed(2));
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
    var departmentId = this.admissionForm.get('departmentId')?.value;
    // if (departmentId == 0 || departmentId == null) {
    //   this.admissionForm.get('doctorId')?.patchValue(0);
    //   this.admissionForm.get('doctorName')?.patchValue('');
    //   this.admissionForm.get('doctor')?.patchValue('');
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
    this.admissionForm.get('departmentId')?.patchValue(selectedValue.departmentId);
    this.admissionForm.get('doctorId')?.patchValue(selectedValue.id);
    this.admissionForm.get('doctorName')?.patchValue(this.formatDoctorDisplay(selectedValue));
    this.admissionForm.get('doctor')?.patchValue(selectedValue);

    const consultationFee = Number(selectedValue?.doctorProfile?.consultationFee ?? 0);
    const firstPayment = this.appointmentPayments.at(0);
    firstPayment?.get('visitFee')?.setValue(consultationFee, { emitEvent: true });
    firstPayment?.get('visitFee')?.disable({ emitEvent: false });
    this.getIPDService();
  }

  getdoctor(itemId: string) {
    return this.doctorList.find((option: { id: string; }) => option.id === itemId);
  }

  onDoctorInputCleared(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    if (!inputValue.trim()) {
      this.admissionForm.get('departmentId')?.patchValue(0);
      this.admissionForm.get('doctorId')?.patchValue(0);
      this.admissionForm.get('doctorName')?.patchValue('');
      this.admissionForm.get('doctor')?.patchValue('');
      const firstPayment = this.appointmentPayments.at(0);
      firstPayment?.get('visitFee')?.enable({ emitEvent: false });
      firstPayment?.get('visitFee')?.setValue(0, { emitEvent: true });
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
    this.admissionForm.get('referrerId')?.patchValue(selectedValue.id);
    this.admissionForm.get('referrerName')?.patchValue(this.formatReferrerDisplay(selectedValue));
    this.admissionForm.get('referrer')?.patchValue(selectedValue);
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
      this.admissionForm.get('referrerId')?.patchValue(null);
      this.admissionForm.get('referrerName')?.patchValue('');
      this.admissionForm.get('referrer')?.patchValue('');
    }
  }


}
