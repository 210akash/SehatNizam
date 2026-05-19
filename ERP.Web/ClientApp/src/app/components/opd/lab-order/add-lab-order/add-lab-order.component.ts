import { Component, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { AppointmentService } from '../../appointment/appointment.service';
import { DepartmentService } from '../../../department/department.service';
import { AppointmentTypeService } from '../../appointment-type/appointment-type.service';
import { PriorityLevelService } from '../../prioritylevel/prioritylevel.service';
import { VisitTypeService } from '../../visit-type/visit-type.service';
import { PaymentModeService } from '../../../paymentmode/paymentmode.service';
import { PrimaryOrderService } from '../../../order/primary-order/order.service';
import { PatientService } from '../../patient/patient.service';
import { LabOrderTypeService } from '../../lab-order-type/lab-order-type.service';
import { Observable, of, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, finalize, map, startWith, switchMap } from 'rxjs/operators';
import { CityService } from '../../../hr/city/city.service';
import { Router } from '@angular/router';

export function discountNotExceedRateValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const parent = control.parent;
    if (!parent) return null;
    const rate = parent.get('rate')?.value;
    const discount = control.value;
    if (rate !== null && discount !== null && discount > rate) {
      return { discountExceedsRate: true };
    }
    return null;
  };
}

@Component({
  selector: 'app-add-lab-order',
  templateUrl: './add-lab-order.component.html',
  styleUrls: ['./add-lab-order.component.css'],
  standalone: false
})
export class AddLabOrderComponent implements OnInit, OnDestroy {
  form!: FormGroup;
  isLoading = false;
  isSubmitting = false;
  successMessage = '';
  errorMessage = '';
  cnicInputMask = createMask('99999-9999999-9');
  phoneNoInputMask = createMask('0399-9999999');
  emailInputMask = createMask('*[*{0,50}]@*[*{0,50}].*[*{0,5}]');
  cityList: any;
  minDate = this.toInputDate(new Date());
  patientSearchCtrl = new FormControl<string | any>('');
  filteredPatients$!: Observable<any[]>;
  patientLoading = false;
  cityList: any[] = [];

  // Lookup data
  labOrderTypes: Array<{ id: number; name: string; price: number,serviceId : number }> = [];
  selectedLabOrderTypeId: number | null = null;
  labTestSearchCtrl = new FormControl('');
  activeCategory: string | null = null;
  readonly CATEGORY_ALL = 'all';
  departments: Array<{ id: number; name: string }> = [];
  appointmentTypeList: Array<{ id: number; name: string }> = [];
  priorityLevelList: Array<{ id: number; name: string }> = [];
  visitTypeList: Array<{ id: number; name: string }> = [];
  paymentModesList: Array<{ id: number; name: string }> = [];
  paymentStatusList: Array<{ id: number; name: string }> = [];
  visitTypeList: Array<{ id: number; name: string }> = [];
  labDepartmentId: number | null = null;
  testsByCategory$!: Observable<Array<{ category: string; tests: Array<{ id: number; name: string; service?: { name: string } }> }>>;

  currentProjectId = 1; // TODO: inject ProjectService
  private labOrderSubscriptions: Subscription[] = [];

  constructor(
    private fb: FormBuilder,
    private dialog: MatDialog,
    private appointmentService: AppointmentService,
    private notifications: NotificationsService,
    private departmentService: DepartmentService,
    private appointmentTypeService: AppointmentTypeService,
    private priorityLevelService: PriorityLevelService,
    private visitTypeService: VisitTypeService,
    private paymentModeService: PaymentModeService,
    private primaryOrderService: PrimaryOrderService,
    private patientService: PatientService,
    private labOrderTypeService: LabOrderTypeService,
    private cityService: CityService,
    private router: Router,
  ) { }

  ngOnInit(): void {
    this.buildForm();
    this.setupPatientAutocomplete();
    this.setupCalculations();
    this.loadLookups();
    this.getCityList();
  }

  ngOnDestroy(): void {
    this.labOrderSubscriptions.forEach(sub => sub.unsubscribe());
  }

  get labOrders(): FormArray<FormGroup> {
    return this.form.get('labOrders') as FormArray<FormGroup>;
  }

  /* ── Derived lists ────────────────────────────── */

  get testCategories(): string[] {
    const cats = new Set<string>();
    this.labOrderTypes.forEach(t => {
      const cat = (t.service?.name || 'Uncategorized').trim();
      if (cat) cats.add(cat);
    });
    return Array.from(cats).sort();
  }

  get totalTestCount(): number {
    return this.labOrderTypes.length;
  }

  /* ── Category helpers ─────────────────────────── */

  getCategoryCount(cat: string): number {
    return this.labOrderTypes.filter(
      t => (t.service?.name || 'Uncategorized').trim() === cat
    ).length;
  }

  setCategory(cat: string): void {
    this.activeCategory = cat === this.CATEGORY_ALL ? null : cat;
    this.buildTestsByCategory();
  }

  /* ── Test selection ───────────────────────────── */

  toggleLabOrderType(test: { id: number; name: string; service?: { name: string } }): void {
    if (this.isTestSelected(test.id)) {
      return;
    }
    this.labOrders.push(this.fb.group({
      labOrderTypeId: [test.id, Validators.required],
      clinicalNotes: ['']
    }));
  }

  isTestSelected(id: number): boolean {
    return this.labOrders.controls.some(
      c => Number(c.get('labOrderTypeId')?.value) === Number(id)
    );
  }

  removeTestById(id: number): void {
    const idx = this.labOrders.controls.findIndex(
      c => Number(c.get('labOrderTypeId')?.value) === Number(id)
    );
    if (idx !== -1) {
      this.labOrders.removeAt(idx);
      this.labOrders.updateValueAndValidity();
    }
  }

  /* ── Legacy single-select fallback ─────────────── */

  addLabOrder(): void {
    this.labTestSearchCtrl.setValue('');
    if (!this.selectedLabOrderTypeId) return;
    const exists = this.labOrders.controls.some(
      c => Number(c.get('labOrderTypeId')?.value) === Number(this.selectedLabOrderTypeId)
    );
    if (exists) {
      this.notifications.showNotification('This test is already selected.', 'snack-bar-danger');
      return;
    }
    this.labOrders.push(this.fb.group({
      labOrderTypeId: [this.selectedLabOrderTypeId, Validators.required],
      clinicalNotes: ['']
    }));
    this.selectedLabOrderTypeId = null;
  }

  removeLabOrder(index: number): void {
    this.labOrders.removeAt(index);
  }

  /* ── Patient search ───────────────────────────── */

  displayPatient = (patient: any): string =>
    patient ? `${patient.name}${patient.phoneNo ? ' - ' + patient.phoneNo : ''}` : '';

  onPatientSelected(patient: any): void {
    if (!patient) return;
    this.patientSearchCtrl.setValue(patient, { emitEvent: false });
    this.form.patchValue({ patientId: patient.id });
    const patientGroup = this.form.get('patient') as FormGroup;
    patientGroup.patchValue({
      name: patient.name,
      phoneNo: patient.phoneNo,
      secondaryPhoneNo: patient.secondaryPhoneNo,
      gender: patient.gender || 'male',
      dateOfBirth: patient.dateOfBirth ? this.toInputDate(patient.dateOfBirth) : null,
      age: patient.age,
      cnic: patient.cnic,
      address: patient.address,
      cityId: patient.cityId ?? 1,
      email: patient.email
    });
    this.updateAge(patient.dateOfBirth);
  }

  onInputCleared(event: Event): void {
    const value = (event.target as HTMLInputElement)?.value?.trim() ?? '';
    if (value.length > 0) return;
    this.patientSearchCtrl.setValue('', { emitEvent: false });
    const patientGroup = this.form.get('patient') as FormGroup;
    patientGroup.reset({
      name: '',
      phoneNo: '',
      secondaryPhoneNo: '',
      gender: 'male',
      dateOfBirth: null,
      age: null,
      cityId: 1,
      cnic: '',
      address: '',
      email: '',
      projectId: 0
    });
    this.form.patchValue({ patientId: null });
  }

  onTestSearch(_event: Event): void {
    // Search is reactive via the test-search form control / filteredTestsForChip getter.
    // This handler exists for any inline search-time side effects if needed later.
  }

  /* ── Payment calculations ─────────────────────── */

  calculateTotals(): void {
    const paymentGroup = this.form.get('appointmentPayment') as FormGroup;
    const visitFee = Number(paymentGroup.get('visitFee')?.value) || 0;
    const discount = Number(paymentGroup.get('discount')?.value) || 0;
    const paidAmount = Number(paymentGroup.get('paidAmount')?.value) || 0;
    const totalPayable = Math.max(0, Number((visitFee - discount).toFixed(2)));
    const balanceAmount = Math.max(0, Number((totalPayable - paidAmount).toFixed(2)));
    paymentGroup.get('totalPayable')?.setValue(totalPayable, { emitEvent: false });
    paymentGroup.get('balanceAmount')?.setValue(balanceAmount, { emitEvent: false });
  }

  /* ── Form building ────────────────────────────── */

  private buildForm(): void {
    this.form = this.fb.group({
      appointmentDate: [this.minDate, Validators.required],
      tokenNumber: [''],
      projectId: [this.currentProjectId, Validators.required],
      departmentId: [null, Validators.required],
      appointmentTypeId: [1, Validators.required],
      priorityLevelId: [1, Validators.required],
      visitTypeId: [1],
      doctorId: [null],
      reason: [''],
      confirmationNotes: [''],
      confirmedDate: [null],
      appointmentStatusId: [5, Validators.required],

      patientId: [null],
      patient: this.fb.group({
        name: ['', Validators.required],
        phoneNo: ['', Validators.required],
        secondaryPhoneNo: [''],
        gender: ['male', Validators.required],
        age: [{ value: null, disabled: true }],
        dateOfBirth: [null, Validators.required],
        cnic: [''],
        address: [''],
        cityId: [1, Validators.required],
        email: ['', Validators.email]
      }),

      // This group is kept for global UI but will NOT be sent as a single payment
      appointmentPayment: this.fb.group({
        id: [0],
        appointmentId: [0],
        visitFee: [0, [Validators.required, Validators.min(0)]],
        discount: [0, [Validators.min(0)]],
        totalPayable: [{ value: 0, disabled: true }],
        paidAmount: [0, [Validators.min(0)]],
        balanceAmount: [{ value: 0, disabled: true }],
        paymentModeId: [1, Validators.required],
        paymentStatusId: [0, Validators.required],
        paymentDate: [new Date().toISOString().split('T')[0], Validators.required]
      }),

      labOrders: this.fb.array([])
    });
  }

  /* ── Patient autocomplete ─────────────────────── */

  private setupPatientAutocomplete(): void {
    this.filteredPatients$ = this.patientSearchCtrl.valueChanges.pipe(
      startWith(''),
      debounceTime(300),
      distinctUntilChanged(),
      switchMap((value: string | any) => {
        const term = typeof value === 'string' ? value : value || '';
        if (!term || term.length < 2) return of([]);
        this.patientLoading = true;
        return this.patientService.getPatientByName(term).pipe(
          map((data: any) => data?.item1 ?? data ?? []),
          finalize(() => (this.patientLoading = false))
        );
      })
    );
  }

  getCityList(): void {
    this.cityService.getAllCities({}).subscribe(data => {
      this.cityList = data?.item1 ?? [];
    });
  }

  private setupCalculations(): void {
    const patientGroup = this.form.get('patient') as FormGroup;
    patientGroup.get('dateOfBirth')?.valueChanges.subscribe((dob) => this.updateAge(dob));

    const paymentGroup = this.form.get('appointmentPayment') as FormGroup;
    paymentGroup.get('visitFee')?.valueChanges.subscribe(() => this.calculateTotals());
    paymentGroup.get('discount')?.valueChanges.subscribe(() => this.calculateTotals());
    paymentGroup.get('paidAmount')?.valueChanges.subscribe(() => this.calculateTotals());
  }

  /* ── Category group builder ───────────────────── */

  private buildTestsByCategory(): void {
    const safeLabOrderTypes = Array.isArray(this.labOrderTypes) ? this.labOrderTypes : [];
    const searchTerm = this.labTestSearchCtrl.value?.trim().toLowerCase() || '';
    const categoryFilter = this.activeCategory || 'ALL';

    let tests = safeLabOrderTypes;
    if (searchTerm) {
      tests = tests.filter(t => t.name.toLowerCase().includes(searchTerm));
    }
    if (categoryFilter !== 'ALL') {
      tests = tests.filter(t => (t.service?.name || 'Uncategorized').trim() === categoryFilter);
    }

    const groupsMap = new Map<string, Array<{ id: number; name: string; service?: { name: string } }>>();
    tests.forEach(t => {
      const key = t.service?.name || 'Uncategorized';
      if (!groupsMap.has(key)) groupsMap.set(key, []);
      groupsMap.get(key)!.push(t);
    });

    this.testsByCategory$ = of(
      Array.from(groupsMap.entries())
        .filter(([_, items]) => items.length > 0)
        .map(([category, tests]) => ({ category, tests }))
    );
  }

  /* ── Lookup loading ───────────────────────────── */

  private loadLookups(): void {
    this.isLoading = true;

    this.departmentService.getClinicalDepartment().subscribe({
      next: (res: any) => {
        this.departments = res?.item1 ?? res ?? [];
        const labDepartment = this.departments.find((d) => d.name?.toLowerCase().includes('lab'));
        this.labDepartmentId = labDepartment?.id ?? null;
        if (this.labDepartmentId) {
          this.form.get('departmentId')?.setValue(this.labDepartmentId);
        }
      }
    });

    this.labOrderTypeService.getAllLabOrderTypes({}).subscribe({
      next: (res: any) => {
        const items = res?.item1 ?? [];
        this.labOrderTypes = items.map((item: any) => ({
          id: item.id,
          name: item.name,
          price: item.service?.basePrice ?? 0,
          serviceId: item.serviceId  // ✅ store the underlying service ID
        }));
      },
      error: () => this.labOrderTypes = []
    });

    this.appointmentTypeService.getAllAppointmentType({})
      .then(obs => obs.subscribe((d: any) => this.appointmentTypeList = d?.item1 ?? []));

    this.priorityLevelService.getAllPriorityLevel({})
      .then(obs => obs.subscribe((d: any) => this.priorityLevelList = d?.item1 ?? []));

    this.paymentModeService.getAllPaymentModes({})
      .subscribe((d: any) => this.paymentModesList = d?.item1 ?? []);

    this.primaryOrderService.getAllOrderStatus()
      .then(obs => obs.subscribe((d: any) => this.paymentStatusList = d ?? []));

    this.visitTypeService.getAllVisitType({})
      .then(obs => obs.subscribe((d: any) => this.visitTypeList = d?.item1 ?? []));

    this.isLoading = false;
  }

  addLabOrder(): void {
     const raw = this.form.getRawValue();
    if (!this.selectedLabOrderTypeId) return;

    const selectedTest = this.labOrderTypes.find(t => t.id === this.selectedLabOrderTypeId);
    if (!selectedTest) {
      this.notifications.showNotification('Selected test not found', 'snack-bar-danger');
      return;
    }

    const exists = this.labOrders.controls.some(
      c => Number(c.get('labOrderTypeId')?.value) === selectedTest.id
    );
    if (exists) {
      this.notifications.showNotification('This test is already selected.', 'snack-bar-danger');
      return;
    }

    const group = this.fb.group({
      labOrderTypeId: [selectedTest.id, Validators.required],
      clinicalNotes: [''],
      testName: [{ value: selectedTest.name, disabled: true }],
      rate: [{ value: selectedTest.price, disabled: true }],
      discount: [0, [Validators.min(0), discountNotExceedRateValidator()]],
      amount: [{ value: selectedTest.price, disabled: true }],
      serviceId: [selectedTest.serviceId], 
    });

    const discountControl = group.get('discount');
    const rateControl = group.get('rate');
    const amountControl = group.get('amount');

    const sub = discountControl?.valueChanges.subscribe(() => {
      let rate = rateControl?.value || 0;
      let discount = discountControl?.value || 0;
      if (discount > rate) {
        discount = rate;
        discountControl?.setValue(discount, { emitEvent: false });
      }
      const newAmount = rate - discount;
      amountControl?.setValue(newAmount, { emitEvent: false });
      this.updateTotalVisitFee();
    });

    if (sub) this.labOrderSubscriptions.push(sub);

    this.labOrders.push(group);
    this.updateTotalVisitFee();
    this.selectedLabOrderTypeId = null;
  }

  removeLabOrder(index: number): void {
    if (this.labOrderSubscriptions[index]) {
      this.labOrderSubscriptions[index].unsubscribe();
      this.labOrderSubscriptions.splice(index, 1);
    }
    this.labOrders.removeAt(index);
    this.updateTotalVisitFee();
  }

  private updateTotalVisitFee(): void {
    const totalAmount = this.labOrders.controls.reduce(
      (sum, group) => sum + (group.get('amount')?.value || 0), 0
    );
    this.form.get('appointmentPayment.visitFee')?.setValue(totalAmount);
  }

  onPatientSelected(patient: any): void {
    if (!patient) return;
    this.patientSearchCtrl.setValue(patient, { emitEvent: false });
    this.form.patchValue({ patientId: patient.id });

    const patientGroup = this.form.get('patient') as FormGroup;
    patientGroup.patchValue({
      name: patient.name,
      phoneNo: patient.phoneNo,
      secondaryPhoneNo: patient.secondaryPhoneNo,
      gender: patient.gender || 'male',
      dateOfBirth: patient.dateOfBirth ? this.toInputDate(patient.dateOfBirth) : null,
      age: patient.age,
      cnic: patient.cnic,
      address: patient.address,
      cityId: patient.cityId ?? 1,
      email: patient.email
    });
    this.appointmentTypeService.getAllAppointmentType({}).then((obs) =>
      obs.subscribe((d: any) => this.appointmentTypeList = d?.item1 ?? [])
    );
    this.priorityLevelService.getAllPriorityLevel({}).then((obs) =>
      obs.subscribe((d: any) => this.priorityLevelList = d?.item1 ?? [])
    );
    this.paymentModeService.getAllPaymentModes({}).subscribe(
      (d: any) => this.paymentModesList = d?.item1 ?? []
    );
    this.primaryOrderService.getAllOrderStatus().then((obs) =>
      obs.subscribe((d: any) => this.paymentStatusList = d ?? [])
    );
    this.visitTypeService.getAllVisitType({}).then((obs) =>
      obs.subscribe((d: any) => this.visitTypeList = d?.item1 ?? [])
    );
    this.cityService.getAllCities({}).subscribe(obs => { this.cityList = obs.item1; });
  }

  calculateTotals(): void {
    const paymentGroup = this.form.get('appointmentPayment') as FormGroup;
    const visitFee = Number(paymentGroup.get('visitFee')?.value) || 0;
    const discount = Number(paymentGroup.get('discount')?.value) || 0;
    const paidAmount = Number(paymentGroup.get('paidAmount')?.value) || 0;

    const totalPayable = Math.max(0, Number((visitFee - discount).toFixed(2)));
    const balanceAmount = Math.max(0, Number((totalPayable - paidAmount).toFixed(2)));

    paymentGroup.get('totalPayable')?.setValue(totalPayable, { emitEvent: false });
    paymentGroup.get('balanceAmount')?.setValue(balanceAmount, { emitEvent: false });
  }

  onSubmit(): void {
    if (this.isSubmitting) return;
    this.errorMessage = '';
    this.successMessage = '';
    if (this.form.invalid || this.labOrders.length === 0) {
      this.form.markAllAsTouched();
      this.form.get('patient')?.markAllAsTouched();
      this.notifications.showNotification(
        'Please complete required fields and add at least one lab test.', 'snack-bar-danger'
      );
      return;
    }

    this.isSubmitting = true;
    const command = this.buildCommand();

    this.appointmentService.saveAppointment(command).subscribe({
      next: (res: any) => {
        this.isSubmitting = false;
        if (res?.Status === 200) {
          this.notifications.showNotification(res?.Data || 'Direct Lab Order Saved Successfully!', 'snack-bar-success');
        this.router.navigate(['/appointment']);
        } else {
          this.notifications.showNotification(
            res?.Message || 'Unable to save lab order.', 'snack-bar-danger'
          );
        }
      },
      error: (error: any) => {
        this.isSubmitting = false;
        const msg = error?.error?.Message || 'An unexpected error occurred.';
        this.notifications.showNotification(msg, 'snack-bar-danger');
      }
    });
  }

  onCancel(): void {
    if (this.dialog) {
      this.dialog.closeAll();
      return;
    }
    const canGoBack = window.history.length > 1;
    if (canGoBack) {
      window.history.back();
    }
  }

  /* ── Command / payload ────────────────────────── */

  buildCommand(): any {
    const raw = this.form.getRawValue();
    const globalPayment = raw.appointmentPayment;

    // Create one payment per lab test
    const payments = raw.labOrders.map((order: any) => ({
      visitFee: order.amount,
      discount: order.discount,
      totalPayable: order.amount,
      paidAmount: 0,
      paymentModeId: globalPayment.paymentModeId,
      paymentStatusId: 3,
      serviceId: order.serviceId
    }));

    return {
      appointmentDate: raw.appointmentDate,
      tokenNumber: raw.tokenNumber,
      projectId: raw.projectId,
      departmentId: raw.departmentId,
      appointmentTypeId: raw.appointmentTypeId,
      priorityLevelId: raw.priorityLevelId,
      visitTypeId: raw.visitTypeId,
      doctorId: raw.doctorId || null,
      reason: raw.reason,
      confirmationNotes: raw.confirmationNotes,
      confirmedDate: raw.confirmedDate || new Date().toISOString(),
      appointmentStatusId: raw.appointmentStatusId,
      patientId: raw.patientId,
      patient: raw.patientId ? null : {
        name: raw.patient.name,
        phoneNo: raw.patient.phoneNo,
        secondaryPhoneNo: raw.patient.secondaryPhoneNo,
        gender: raw.patient.gender,
        dateOfBirth: raw.patient.dateOfBirth,
        cnic: raw.patient.cnic,
        address: raw.patient.address,
        cityId: raw.patient.cityId,
        email: raw.patient.email
      },
      appointmentPayment: payments,
      labOrders: raw.labOrders.map((x: any) => ({
        labOrderTypeId: x.labOrderTypeId,
        clinicalNotes: x.clinicalNotes || ''
      })),
      radiologyOrders: []
    };
  }

  /* ── Reset ────────────────────────────────────── */

  resetForm(): void {
    this.form.reset();
    this.labOrders.clear();
    this.patientSearchCtrl.setValue('');
    this.form.get('appointmentDate')?.setValue(this.minDate);
    this.form.get('appointmentPayment.paymentDate')?.setValue(new Date().toISOString().split('T')[0]);
    this.form.get('projectId')?.setValue(this.currentProjectId);
  }

  /* ── Helpers ──────────────────────────────────── */

  getLabOrderTypeName(id: number): string {
    return this.labOrderTypes.find(x => x.id === id)?.name || '-';
  }

  private updateAge(dob: string | Date | null): void {
    const age = this.calculateAge(dob);
    (this.form.get('patient') as FormGroup).get('age')?.setValue(age, { emitEvent: false });
  }

  private calculateAge(dob: string | Date | null): number | null {
    if (!dob) return null;
    const birthDate = new Date(dob);
    const diff = Date.now() - birthDate.getTime();
    return Math.abs(new Date(diff).getUTCFullYear() - 1970);
  }

  private toInputDate(date: Date | string): string {
    const d = new Date(date);
    const month = `${d.getMonth() + 1}`.padStart(2, '0');
    const day = `${d.getDate()}`.padStart(2, '0');
    return `${d.getFullYear()}-${month}-${day}`;
  }
}