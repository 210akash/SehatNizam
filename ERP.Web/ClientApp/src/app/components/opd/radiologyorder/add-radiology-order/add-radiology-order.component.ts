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
import { PatientService } from '../../patient/patient.service';
import { Observable, of, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, finalize, map, startWith, switchMap } from 'rxjs/operators';
import { CityService } from '../../../hr/city/city.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { ReferrerService } from '../../referrer/referrer.service';
import { RadiologyTypeService } from '../../radiologytype/radiologytype.service';
import { RadiologyOrderService } from '../radiologyorder.service';

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
  selector: 'app-add-radiology-order',
  templateUrl: './add-radiology-order.component.html',
  styleUrls: ['./add-radiology-order.component.css'],
  standalone: false
})
export class AddRadiologyOrderComponent implements OnInit, OnDestroy {
  form!: FormGroup;
  isLoading = false;
  isSubmitting = false;
  minDate = this.toInputDate(new Date());
  patientSearchCtrl = new FormControl<string | any>('');
  filteredPatients$!: Observable<any[]>;
  patientLoading = false;
  cityList: any[] = [];

  // Lookup data
  radiologyOrderTypes: Array<{ id: number; name: string; price: number,serviceId : number }> = [];
  selectedRadiologyOrderTypeId: number | null = null;
  departments: Array<{ id: number; name: string }> = [];
  appointmentTypeList: Array<{ id: number; name: string }> = [];
  priorityLevelList: Array<{ id: number; name: string }> = [];
  paymentModesList: Array<{ id: number; name: string }> = [];
  paymentStatusList: Array<{ id: number; name: string }> = [];
  visitTypeList: Array<{ id: number; name: string }> = [];
  radiologyDepartmentId: number | null = null;

  currentProjectId = 1; // TODO: inject ProjectService
  private radiologyOrderSubscriptions: Subscription[] = [];
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
    private cityService: CityService,
    private radiologyTypeService: RadiologyTypeService, 
    private router: Router,
    private route: ActivatedRoute,
    private referrerService: ReferrerService,
    private radiologyOrderService: RadiologyOrderService, 

  ) { }

  ngOnInit(): void {
    this.buildForm();
    this.setupPatientAutocomplete();
    this.setupCalculations();
    this.loadLookups();
    this.getCityList();
    // this.patchEditData();
    this.setupRadiologyOrderTypeWatcher();
  }

  ngOnDestroy(): void {
    this.radiologyOrderSubscriptions.forEach(sub => sub.unsubscribe());
  }

  get radiologyOrders(): FormArray<FormGroup> {
    return this.form.get('radiologyOrders') as FormArray<FormGroup>;
  }

  displayPatient = (patient: any): string => {
    if (!patient) {
      return '';
    }

    const master = patient.patientMaster ?? patient;
    const parts = [patient.mrn, master.name, master.phoneNo].filter(Boolean);
    return parts.join(' - ');
  };

  private getPatientMaster(patient: any): any {
    return patient?.patientMaster ?? patient ?? {};
  }

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

      radiologyOrders: this.fb.array([])
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
          : (typeof value?.name === 'string'
            ? value.name.trim()
            : (typeof value?.patientMaster?.name === 'string' ? value.patientMaster.name.trim() : ''));
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
        const radiologyDepartment = this.departments.find((d) => d.name?.toLowerCase().includes('radiology'));
        this.radiologyDepartmentId = radiologyDepartment?.id ?? null;
        if (this.radiologyDepartmentId) {
          this.form.get('departmentId')?.setValue(this.radiologyDepartmentId);
        }
      }
    });

    this.radiologyTypeService.getAllRadiologyTypes({}).subscribe({
      next: (res: any) => {
        const items = res?.item1 ?? [];
        this.radiologyOrderTypes = items.map((item: any) => ({
          id: item.id,
          name: item.name,
          price: item.service?.basePrice ?? 0,
          serviceId: item.serviceId  // ✅ store the underlying service ID
        }));
      },
      error: () => this.radiologyOrderTypes = []
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

  addRadiologyOrder(): void {
     const raw = this.form.getRawValue();
    if (!this.selectedRadiologyOrderTypeId) return;

    const selectedTest = this.radiologyOrderTypes.find(t => t.id === this.selectedRadiologyOrderTypeId);
    if (!selectedTest) {
      this.notifications.showNotification('Selected test not found', 'snack-bar-danger');
      return;
    }

    const exists = this.radiologyOrders.controls.some(
      c => Number(c.get('radiologyOrderTypeId')?.value) === selectedTest.id
    );
    if (exists) {
      this.notifications.showNotification('This test is already selected.', 'snack-bar-danger');
      return;
    }

    const group = this.fb.group({
      radiologyOrderTypeId: [selectedTest.id, Validators.required],
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

    if (sub) this.radiologyOrderSubscriptions.push(sub);

    this.radiologyOrders.push(group);
    this.updateTotalVisitFee();
    this.selectedRadiologyOrderTypeId = null;
  }

  removeRadiologyOrder(index: number): void {
    if (this.radiologyOrderSubscriptions[index]) {
      this.radiologyOrderSubscriptions[index].unsubscribe();
      this.radiologyOrderSubscriptions.splice(index, 1);
    }
    this.radiologyOrders.removeAt(index);
    this.updateTotalVisitFee();
  }

  private updateTotalVisitFee(): void {
    const totalAmount = this.radiologyOrders.controls.reduce(
      (sum, group) => sum + (group.get('amount')?.value || 0), 0
    );
    this.form.get('appointmentPayment.visitFee')?.setValue(totalAmount);
  }

  onPatientSelected(patient: any): void {
    if (!patient) return;

    const master = this.getPatientMaster(patient);
    this.patientSearchCtrl.setValue(master, { emitEvent: false });
    this.form.patchValue({ patientId: patient.id });

    const patientGroup = this.form.get('patient') as FormGroup;
    patientGroup.patchValue({
      name: master.name,
      phoneNo: master.phoneNo,
      secondaryPhoneNo: master.secondaryPhoneNo,
      gender: master.gender || 'male',
      dateOfBirth: master.dateOfBirth ? this.toInputDate(master.dateOfBirth) : null,
      age: master.age,
      cnic: master.cnic,
      address: master.address,
      cityId: master.cityId ?? 1,
      email: master.email
    });
    this.updateAge(master.dateOfBirth);
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
    if (this.form.invalid || this.radiologyOrders.length === 0) {
      this.form.markAllAsTouched();
      this.notifications.showNotification('Please complete required fields and add at least one radiology test.', 'snack-bar-danger');
      return;
    }

    this.isSubmitting = true;
    const command = this.buildCommand();

    this.appointmentService.saveAppointment(command).subscribe({
      next: (res: any) => {
        this.isSubmitting = false;
        if (res?.Status === 200) {
          this.notifications.showNotification(res?.Data || 'Direct Radiology Order Saved Successfully!', 'snack-bar-success');
        this.router.navigate(['/radiologyorder']);
        } else {
          this.notifications.showNotification(res?.Message || 'Unable to save radiology order.', 'snack-bar-danger');
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

    // Create one payment per radiology test
    const payments = raw.radiologyOrders.map((order: any) => ({
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
      patientId: raw.patientId || null,
      patient: {
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
      radiologyOrders: raw.radiologyOrders.map((x: any) => ({
        radiologyTypeId: x.radiologyOrderTypeId,
        clinicalNotes: x.clinicalNotes || '',
        statusId: x.statusId || 5,
      })),
    };
  }

  resetForm(): void {
    this.form.reset();
    this.radiologyOrders.clear();
    this.patientSearchCtrl.setValue('');
    this.form.get('appointmentDate')?.setValue(this.minDate);
    this.form.get('appointmentPayment.paymentDate')?.setValue(new Date().toISOString().split('T')[0]);
    this.form.get('projectId')?.setValue(this.currentProjectId);
  }

  getRadiologyOrderTypeName(id: number): string {
    return this.radiologyOrderTypes.find(x => x.id === id)?.name || '-';
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
    const master = this.getPatientMaster(patient);

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
      name: master.name || patient.name || '',
      phoneNo: master.phoneNo || patient.phoneNo || '',
      secondaryPhoneNo: master.secondaryPhoneNo || patient.secondaryPhoneNo || '',
      gender: master.gender || patient.gender || 'male',
      age: master.age ?? patient.age ?? null,
      dateOfBirth: (master.dateOfBirth || patient.dateOfBirth) ? this.toInputDate(master.dateOfBirth || patient.dateOfBirth) : null,
      cnic: master.cnic || patient.cnic || '',
      address: master.address || patient.address || '',
      cityId: master.cityId ?? patient.cityId ?? 1,
      email: master.email || patient.email || ''
    });

    if (patient.id) {
      this.patientSearchCtrl.setValue(master.name ? master : patient, { emitEvent: false });
    }

    const payments = element.appointmentPayments || appointment.appointmentPayments || [];
    this.radiologyOrders.clear();

    (element.radiologyOrders || []).forEach((order: any) => {
      const radiologyOrderType = this.radiologyOrderTypes.find(t => t.id === order.radiologyOrderTypeId);
      const testName = radiologyOrderType?.name || order.radiologyOrderType?.name || order.testName || '';
      const rate = radiologyOrderType?.price ?? order.rate ?? order.radiologyOrderType?.service?.basePrice ?? 0;
      const serviceId = radiologyOrderType?.serviceId ?? order.serviceId ?? order.radiologyOrderType?.serviceId ?? 0;

      const group = this.fb.group({
        radiologyOrderTypeId: [order.radiologyOrderTypeId, Validators.required],
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
      if (sub) this.radiologyOrderSubscriptions.push(sub);
      this.radiologyOrders.push(group);
    });

    this.updateTotalVisitFee();
  }

  private setupRadiologyOrderTypeWatcher(): void {
    this.radiologyTypeService.getAllRadiologyTypes({}).subscribe({
      next: (res: any) => {
        const items = res?.item1 ?? [];
        this.radiologyOrderTypes = items.map((item: any) => ({
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
      this.router.navigate(['/radiologyorder']);
      return;
    }

    // When opened as a page, navigate back to the Radiologyorder list.
    const canGoBack = window.history.length > 1;
    if (canGoBack) {
      window.history.back();
    } else {
      this.router.navigate(['/radiologyorder']);
    }
  }
}