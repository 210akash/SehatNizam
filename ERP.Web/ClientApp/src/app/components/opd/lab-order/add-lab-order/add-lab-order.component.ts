import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
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
import { Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, finalize, map, startWith, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-add-lab-order',
  templateUrl: './add-lab-order.component.html',
  styleUrls: ['./add-lab-order.component.css'],
  standalone: false
})
export class AddLabOrderComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;
  isSubmitting = false;
  minDate = this.toInputDate(new Date());
  patientSearchCtrl = new FormControl<string | any>('');
  filteredPatients$!: Observable<any[]>;
  patientLoading = false;
  labOrderTypes: Array<{ id: number; name: string }> = [];
  selectedLabOrderTypeId: number | null = null;
  departments: Array<{ id: number; name: string }> = [];
  appointmentTypeList: Array<{ id: number; name: string }> = [];
  priorityLevelList: Array<{ id: number; name: string }> = [];
  paymentModesList: Array<{ id: number; name: string }> = [];
  paymentStatusList: Array<{ id: number; name: string; title?: string }> = [];
  labDepartmentId: number | null = null;

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
    private labOrderTypeService: LabOrderTypeService
  ) { }

  ngOnInit(): void {
    this.buildForm();
    this.setupPatientAutocomplete();
    this.setupCalculations();
    this.loadLookups();
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
      departmentId: [null, Validators.required],
      appointmentTypeId: [1, Validators.required],
      priorityLevelId: [1, Validators.required],
      visitTypeId: [1],
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
        paymentDate: [this.minDate, Validators.required]
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

  private setupCalculations(): void {
    const patientGroup = this.form.get('patient') as FormGroup;
    patientGroup.get('dateOfBirth')?.valueChanges.subscribe((dob) => this.updateAge(dob));

    const paymentGroup = this.form.get('appointmentPayment') as FormGroup;
    paymentGroup.valueChanges.subscribe(() => this.calculateTotals());
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
      next: (res: any) => this.labOrderTypes = res?.item1 ?? [],
      error: () => this.labOrderTypes = []
    });
    this.appointmentTypeService.getAllAppointmentType({}).then((obs) => obs.subscribe((d: any) => this.appointmentTypeList = d?.item1 ?? []));
    this.priorityLevelService.getAllPriorityLevel({}).then((obs) => obs.subscribe((d: any) => this.priorityLevelList = d?.item1 ?? []));
    this.paymentModeService.getAllPaymentModes({}).subscribe((d: any) => this.paymentModesList = d?.item1 ?? []);
    this.primaryOrderService.getAllOrderStatus().then((obs) => obs.subscribe((d: any) => this.paymentStatusList = d ?? []));
    this.visitTypeService.getAllVisitType({}).then((obs) => obs.subscribe());
    this.isLoading = false;
  }

  addLabOrder(): void {
    if (!this.selectedLabOrderTypeId) return;
    const exists = this.labOrders.controls.some(c => Number(c.get('labOrderTypeId')?.value) === Number(this.selectedLabOrderTypeId));
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
    this.appointmentService.saveAppointment(command).subscribe({
      next: (res: any) => {
        this.isSubmitting = false;
        if (res?.Status === 200) {
          this.notifications.showNotification(res?.Data || 'Direct Lab Order Saved Successfully!', 'snack-bar-success');
          this.dialog.closeAll();
          this.resetForm();
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
    return {
      appointmentDate: raw.appointmentDate,
      tokenNumber: raw.tokenNumber,
      departmentId: raw.departmentId,
      appointmentTypeId: raw.appointmentTypeId,
      priorityLevelId: raw.priorityLevelId,
      visitTypeId: raw.visitTypeId,
      appointmentStatusId: raw.appointmentStatusId,
      patientId: raw.patientId,
      patient: raw.patientId ? null : raw.patient,
      appointmentPayment: raw.appointmentPayment,
      labOrders: raw.labOrders.map((x: { labOrderTypeId: number; clinicalNotes: string }) => ({
        labOrderTypeId: x.labOrderTypeId,
        clinicalNotes: x.clinicalNotes || ''
      }))
    };
  }

  resetForm(): void {
    this.form.reset();
    this.labOrders.clear();
    this.patientSearchCtrl.setValue('');
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
}
