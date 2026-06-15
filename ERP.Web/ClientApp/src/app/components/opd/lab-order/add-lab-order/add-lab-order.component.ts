import { Component, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { NotificationsService } from '../../../../Service/notification.service';
import { AppointmentService } from '../../appointment/appointment.service';
import { DepartmentService } from '../../../department/department.service';
import { AppointmentTypeService } from '../../appointment-type/appointment-type.service';
import { PriorityLevelService } from '../../prioritylevel/prioritylevel.service';
import { VisitTypeService } from '../../visit-type/visit-type.service';
import { PaymentModeService } from '../../../paymentmode/paymentmode.service';
import { LabOrderService } from '../lab-order.service';
import { PatientService } from '../../patient/patient.service';
import { LabOrderTypeService } from '../../lab-order-type/lab-order-type.service';
import { Observable, of, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, finalize, map, startWith, switchMap } from 'rxjs/operators';
import { CityService } from '../../../hr/city/city.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { ReferrerService } from '../../referrer/referrer.service';

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
  minDate = this.toInputDate(new Date());
  patientSearchCtrl = new FormControl<string | any>('');
  filteredPatients$!: Observable<any[]>;
  patientLoading = false;
  cityList: any[] = [];

  // Lookup data
  labOrderTypes: Array<{ id: number; name: string; price: number,serviceId : number }> = [];
  selectedLabOrderTypeId: number | null = null;
  departments: Array<{ id: number; name: string }> = [];
  appointmentTypeList: Array<{ id: number; name: string }> = [];
  priorityLevelList: Array<{ id: number; name: string }> = [];
  paymentModesList: Array<{ id: number; name: string }> = [];
  paymentStatusList: Array<{ id: number; name: string }> = [];
  visitTypeList: Array<{ id: number; name: string }> = [];
  labDepartmentId: number | null = null;

  currentProjectId = 1; // TODO: inject ProjectService
  private labOrderSubscriptions: Subscription[] = [];
  referrerList : any[] = [];
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
    private patientService: PatientService,
    private labOrderTypeService: LabOrderTypeService,
    private cityService: CityService,
    private router: Router,
    private route: ActivatedRoute,
    private referrerService: ReferrerService,
  ) { }

  ngOnInit(): void {
    this.buildForm();
    this.setupPatientAutocomplete();
    this.setupCalculations();
    this.loadLookups();
    this.getCityList();
    // this.patchEditData();
    this.setupLabOrderTypeWatcher();
  }

  ngOnDestroy(): void {
    this.labOrderSubscriptions.forEach(sub => sub.unsubscribe());
  }

  get labOrders(): FormArray<FormGroup> {
    return this.form.get('labOrders') as FormArray<FormGroup>;
  }

  displayPatient = (patient: any): string =>
    patient ? `${patient.name}${patient.phoneNo ? ' - ' + patient.phoneNo : ''}` : '';

  private buildForm(): void {
    this.form = this.fb.group({
      appointmentDate: [this.minDate, Validators.required],
      tokenNumber: [''],
      projectId: [this.currentProjectId, Validators.required],
      departmentId: [null, Validators.required],
      referrerId: [null],
      referrerName: [''],
      referrer: [''],
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

  private setupPatientAutocomplete(): void {
    this.filteredPatients$ = this.patientSearchCtrl.valueChanges.pipe(
      startWith(''),
      debounceTime(300),
      distinctUntilChanged(),
      switchMap((value: string | any) => {
        const term = typeof value === 'string'
          ? value.trim()
          : (typeof value?.name === 'string' ? value.name.trim() : '');
        if (term.length < 2) return of([]);
        this.patientLoading = true;
        return this.patientService.getPatientByName(term).pipe(
          finalize(() => (this.patientLoading = false))
        );
      })
    );
  }


getCityList(): void {
  this.cityService.getAllCities({}).subscribe(data => {
    this.cityList = data.item1;

    const lahoreCity = this.cityList.find(
      (city: any) => city.name?.toLowerCase() === 'lahore'
    );

    if (lahoreCity) {
      this.form.get('patient.cityId')?.setValue(lahoreCity.id);
    }
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
      statusId: [5],
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
    this.updateAge(patient.dateOfBirth);
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
    if (this.form.invalid || this.labOrders.length === 0) {
      this.form.markAllAsTouched();
      this.notifications.showNotification('Please complete required fields and add at least one lab test.', 'snack-bar-danger');
      return;
    }

    this.isSubmitting = true;
    const command = this.buildCommand();

    this.appointmentService.saveAppointmentLab(command).subscribe({
      next: (res: any) => {
        this.isSubmitting = false;
        if (res?.Status === 200) {
          this.notifications.showNotification(res?.Data || 'Direct Lab Order Saved Successfully!', 'snack-bar-success');
        this.router.navigate(['/laborder']);
        } else {
          this.notifications.showNotification(res?.Message || 'Unable to save lab order.', 'snack-bar-danger');
        }
      },
      error: (error: any) => {
        this.isSubmitting = false;
        const msg = error?.error?.Message || 'An unexpected error occurred.';
        this.notifications.showNotification(msg, 'snack-bar-danger');
      }
    });
  }

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
      referrerId: raw.referrerId || null,
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
        clinicalNotes: x.clinicalNotes || '',
      })),
      radiologyOrders: []
    };
  }

  resetForm(): void {
    this.form.reset();
    this.labOrders.clear();
    this.patientSearchCtrl.setValue('');
    this.form.get('appointmentDate')?.setValue(this.minDate);
    this.form.get('appointmentPayment.paymentDate')?.setValue(new Date().toISOString().split('T')[0]);
    this.form.get('projectId')?.setValue(this.currentProjectId);
  }

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

  private patchEditData(): void {
    const element = history.state?.element;
    if (!element) return;

    const appointment = element.appointment || {};
    const patient = appointment.patient || element.patient || {};

    this.form.patchValue({
      appointmentDate: appointment.appointmentDate ? this.toInputDate(appointment.appointmentDate) : this.minDate,
      tokenNumber: appointment.tokenNumber || '',
      projectId: appointment.projectId || this.currentProjectId,
      departmentId: appointment.departmentId || null,
      referrerId: appointment.referrerId || null,
      referrerName: appointment.referrer?.name + ' ' +appointment.referrer?.hospital   || '',
      referrer: appointment.referrer || null,
      appointmentTypeId: appointment.appointmentTypeId || 1,
      priorityLevelId: appointment.priorityLevelId || 1,
      visitTypeId: appointment.visitTypeId || 1,
      doctorId: appointment.doctorId || null,
      reason: appointment.reason || '',
      confirmationNotes: appointment.confirmationNotes || '',
      confirmedDate: appointment.confirmedDate || null,
      appointmentStatusId: appointment.appointmentStatusId || 5,
      patientId: patient.id || null
    });

    const patientGroup = this.form.get('patient') as FormGroup;
    patientGroup.patchValue({
      name: patient.name || '',
      phoneNo: patient.phoneNo || '',
      secondaryPhoneNo: patient.secondaryPhoneNo || '',
      gender: patient.gender || 'male',
      age: patient.age || null,
      dateOfBirth: patient.dateOfBirth ? this.toInputDate(patient.dateOfBirth) : null,
      cnic: patient.cnic || '',
      address: patient.address || '',
      cityId: patient.cityId ?? 1,
      email: patient.email || ''
    });

    if (patient.id) {
      this.patientSearchCtrl.setValue(patient, { emitEvent: false });
    }

    const payments = element.appointmentPayments || appointment.appointmentPayments || [];
    this.labOrders.clear();

    (element.labOrders || []).forEach((order: any) => {
      const labOrderType = this.labOrderTypes.find(t => t.id === order.labOrderTypeId);
      const testName = labOrderType?.name || order.labOrderType?.name || order.testName || '';
      const rate = labOrderType?.price ?? order.rate ?? order.labOrderType?.service?.basePrice ?? 0;
      const serviceId = labOrderType?.serviceId ?? order.serviceId ?? order.labOrderType?.serviceId ?? 0;

      const group = this.fb.group({
        labOrderTypeId: [order.labOrderTypeId, Validators.required],
        clinicalNotes: [order.clinicalNotes || ''],
        testName: [{ value: testName, disabled: true }],
        rate: [{ value: rate, disabled: true }],
        discount: [order.discount || 0, [Validators.min(0), discountNotExceedRateValidator()]],
        amount: [{ value: order.amount ?? rate, disabled: true }],
        serviceId: [serviceId]
      });

      const discountControl = group.get('discount');
      const amountControl = group.get('amount');
      const sub = discountControl?.valueChanges.subscribe(() => {
        let r = rate;
        let d = discountControl?.value || 0;
        if (d > r) {
          d = r;
          discountControl?.setValue(d, { emitEvent: false });
        }
        amountControl?.setValue(r - d, { emitEvent: false });
        this.updateTotalVisitFee();
      });
      if (sub) this.labOrderSubscriptions.push(sub);
      this.labOrders.push(group);
    });

    this.updateTotalVisitFee();
  }

  private setupLabOrderTypeWatcher(): void {
    this.labOrderTypeService.getAllLabOrderTypes({}).subscribe({
      next: (res: any) => {
        const items = res?.item1 ?? [];
        this.labOrderTypes = items.map((item: any) => ({
          id: item.id,
          name: item.name,
          price: item.service?.basePrice ?? 0,
          serviceId: item.serviceId
        }));
      }
    });
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
      this.form.get('referrerId')?.patchValue(selectedValue.id);
      this.form.get('referrerName')?.patchValue(this.formatReferrerDisplay(selectedValue));
      this.form.get('referrer')?.patchValue(selectedValue);
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
        this.form.get('referrerId')?.patchValue(null);
        this.form.get('referrerName')?.patchValue('');
        this.form.get('referrer')?.patchValue('');
      }
    }

    onCancel(): void {
    if (this.dialog) {
      this.dialog.closeAll();
      this.router.navigate(['/laborder']);
      return;
    }

    // When opened as a page, navigate back to the Laborder list.
    const canGoBack = window.history.length > 1;
    if (canGoBack) {
      window.history.back();
    } else {
      this.router.navigate(['/laborder']);
    }
  }
}