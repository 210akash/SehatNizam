import { Component, Inject, OnInit, Optional } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, finalize, map, startWith, switchMap } from 'rxjs/operators';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { TriageService } from '../triage.service';
import { TriageCategoryService } from '../../triage-category/triage-category.service';
import { PriorityLevelService } from '../../prioritylevel/prioritylevel.service';
import { SugarTypeService } from '../../sugar-type/sugar-type.service';
import { AppointmentService } from '../../appointment/appointment.service';

@Component({
  selector: 'app-create-triage',
  templateUrl: './create-triage.component.html',
  styleUrls: ['./create-triage.component.css'], standalone: false
})
export class CreateTriageComponent implements OnInit {
  createTriageForm!: FormGroup;
  isLoading = false;
  isQueueLoading = false;
  triageCategories: any[] = [];
  triagePriorities: any[] = [];
  sugarTypes: any[] = [];
  isEditMode = false;
  hasUnsavedChanges = false;
  suppressDirtyTracking = false;
  appointments: any[] = [];
  selectedAppointment: any = null;
  selectedIndex = 0;
  queueCount = 0;
  checkedCount = 0;
  appointmentSearchCtrl = new FormControl<string | any>('');
  filteredAppointments$!: Observable<any[]>;
  appointmentLoading = false;
  private initialElement: any = null;

  constructor(
    private notificationsService: NotificationsService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private constantService: ConstantService,
    private triageService: TriageService,
    private triageCategoryService: TriageCategoryService,
    private priorityLevelService: PriorityLevelService,
    private sugarTypeService: SugarTypeService,
    private appointmentService: AppointmentService,
    private router: Router,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: { element: any } | null
  ) { }

  ngOnInit(): void {
    this.initialElement = this.data?.element ?? history.state?.element ?? null;
    this.buildForm();
    this.registerBmiCalculation();
    this.setupDirtyTracking();
    this.setupAppointmentAutocomplete();
    this.loadDropdowns();
    this.loadAppointmentQueue();
  }

  get f() {
    return this.createTriageForm.controls;
  }

  private buildForm() {
    this.createTriageForm = this.formBuilder.group({
      id: [0],
      appointmentId: [null, Validators.required],
      nurseId: [null],
      temperature: [null],
      pulse: [null],
      systolicBp: [null],
      diastolicBp: [null],
      spo2: [null],
      weight: [null],
      heightFeet: [null],
      heightInches: [null],
      heightCm: [null],
      bmi: [{ value: null, disabled: true }],
      bloodSugar: [null],
      sugarTypeId: [null, Validators.required],
      triagePriorityId: [null, Validators.required],
      chiefComplaint: [''],
      allergies: [''],
      medications: [''],
      notes: [''],
      triageScore: [null],
      triageCategoryId: [null, Validators.required]
    });
  }

  private setupDirtyTracking() {
    this.createTriageForm.valueChanges.subscribe(() => {
      if (!this.suppressDirtyTracking) {
        this.hasUnsavedChanges = true;
      }
    });
  }

  private setupAppointmentAutocomplete() {
    this.filteredAppointments$ = this.appointmentSearchCtrl.valueChanges.pipe(
      startWith(''),
      debounceTime(300),
      distinctUntilChanged(),
      switchMap((value: string | any) => {
        const term = typeof value === 'string'
          ? value.trim()
          : String(value?.tokenNumber ?? value?.id ?? '').trim();

        if (!term) {
          return of([]);
        }

        this.appointmentLoading = true;
        return this.appointmentService.getAppointmentByToken(term).pipe(
          map((data: any) => data?.item1 ?? data ?? []),
          finalize(() => (this.appointmentLoading = false))
        );
      })
    );
  }

  private registerBmiCalculation() {
    this.createTriageForm.valueChanges.subscribe(() => {
      const weight = Number(this.createTriageForm.get('weight')?.value);
      const heightCm = Number(this.createTriageForm.get('heightCm')?.value);
      if (!!weight && !!heightCm) {
        const heightM = heightCm / 100;
        const bmi = weight / (heightM * heightM);
        this.createTriageForm.get('bmi')?.setValue(Number(bmi.toFixed(2)), { emitEvent: false });
      }
    });
  }

  async loadDropdowns() {
    await this.loadTriageCategories();
    await this.loadTriagePriorities();
    await this.loadSugarTypes();
  }

  private async loadTriageCategories() {
    const filter: any = { pagingData: { currentPage: 0, take: 1000 } };
    (await this.triageCategoryService.getAllTriageCategory(filter)).subscribe(data => {
      this.triageCategories = data?.item1 ?? data ?? [];
    });
  }

  private async loadTriagePriorities() {
    const filter: any = { pagingData: { currentPage: 0, take: 1000 } };
    (await this.priorityLevelService.getAllPriorityLevel(filter)).subscribe(data => {
      this.triagePriorities = data?.item1 ?? data ?? [];
    });
  }

  private async loadSugarTypes() {
    const filter: any = { pagingData: { currentPage: 0, take: 1000 } };
    (await this.sugarTypeService.getAllSugarType(filter)).subscribe(data => {
      this.sugarTypes = data?.item1 ?? data ?? [];
    });
  }

  async loadAppointmentQueue() {
    this.isQueueLoading = true;

    const today = new Date().toLocaleDateString();
    const filter = {
      code: '',
      fdate: today,
      tdate: today,
      PagingData: { currentPage: 0, take: 100 }
    };

    this.appointmentService.getAllAppointments(filter).subscribe({
      next: (data: any) => {
        this.appointments = data?.item1 ?? [];
        this.queueCount = this.appointments.length;

        if (!this.appointments.length) {
          this.selectedAppointment = null;
          this.isQueueLoading = false;
          return;
        }

        const requestedAppointmentId = this.initialElement?.appointmentId;
        const selectedIndex = requestedAppointmentId
          ? Math.max(this.appointments.findIndex((appointment: any) => appointment.id === requestedAppointmentId), 0)
          : 0;

        this.selectAppointment(selectedIndex, false);
        this.isQueueLoading = false;
      },
      error: (error: any) => {
        console.log(error);
        this.notificationsService.showNotification('Unable to load appointment queue.', 'snack-bar-danger');
        this.selectedAppointment = null;
        this.isQueueLoading = false;
      }
    });
  }

  selectAppointment(index: number, promptForUnsavedChanges = true): void {
    if (index < 0 || index >= this.appointments.length) {
      return;
    }

    if (promptForUnsavedChanges && this.hasUnsavedChanges) {
      const shouldContinue = window.confirm('You have unsaved triage changes. Continue without saving?');
      if (!shouldContinue) {
        return;
      }
    }

    this.selectedIndex = index;
    this.selectedAppointment = this.appointments[index];
    this.checkedCount = index;
    this.queueCount = this.appointments.length;
    this.appointmentSearchCtrl.setValue(this.selectedAppointment, { emitEvent: false });
    this.loadTriageForAppointment(this.selectedAppointment.id);
  }

  prevAppointment(): void {
    this.selectAppointment(this.selectedIndex - 1);
  }

  nextAppointment(): void {
    this.selectAppointment(this.selectedIndex + 1);
  }

  private async loadTriageForAppointment(appointmentId: number) {
    this.suppressDirtyTracking = true;
    this.isLoading = true;

    this.resetFormForAppointment(appointmentId);

    const filter = {
      appointmentId,
      PagingData: { currentPage: 0, take: 1 }
    };

    (await this.triageService.getAllTriage(filter)).subscribe({
      next: (data: any) => {
        const triage = data?.item1?.[0];

        if (triage) {
          this.isEditMode = true;
          this.constantService.LoadData(triage, this.createTriageForm);
          if (triage?.bmi != null) {
            this.createTriageForm.get('bmi')?.setValue(triage.bmi, { emitEvent: false });
          }
        } else {
          this.isEditMode = false;
          this.resetFormForAppointment(appointmentId);
        }

        this.hasUnsavedChanges = false;
        this.createTriageForm.markAsPristine();
        this.suppressDirtyTracking = false;
        this.isLoading = false;
      },
      error: (error: any) => {
        console.log(error);
        this.isEditMode = false;
        this.resetFormForAppointment(appointmentId);
        this.hasUnsavedChanges = false;
        this.createTriageForm.markAsPristine();
        this.suppressDirtyTracking = false;
        this.isLoading = false;
      }
    });
  }

  private resetFormForAppointment(appointmentId: number) {
    this.createTriageForm.reset({
      id: 0,
      appointmentId,
      nurseId: null,
      temperature: null,
      pulse: null,
      systolicBp: null,
      diastolicBp: null,
      spo2: null,
      weight: null,
      heightFeet: null,
      heightInches: null,
      heightCm: null,
      bmi: null,
      bloodSugar: null,
      sugarTypeId: null,
      triagePriorityId: null,
      chiefComplaint: '',
      allergies: '',
      medications: '',
      notes: '',
      triageScore: null,
      triageCategoryId: null
    }, { emitEvent: false });
  }

  async saveTriage(moveNext = false) {
    this.isLoading = true;
    this.createTriageForm.get('appointmentId')?.setValue(this.selectedAppointment?.id ?? null);

    if (this.createTriageForm.invalid) {
      this.constantService.markFormGroupTouched(this.createTriageForm);
      this.isLoading = false;
      return;
    }

    const payload = Object.assign({}, this.createTriageForm.getRawValue());

    (await this.triageService.saveTriage(payload)).subscribe({
      next: (data: { Status: number; Message?: string; Data?: string }) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Message || 'Triage Saved Successfully!', 'snack-bar-success');
          this.hasUnsavedChanges = false;
          this.isEditMode = true;
          this.loadTriageForAppointment(this.selectedAppointment.id);

          if (moveNext && this.selectedIndex < this.appointments.length - 1) {
            this.selectAppointment(this.selectedIndex + 1, false);
          }
        } else if (data.Status == 409) {
          this.notificationsService.showNotification(data.Message || 'Record already exists!', 'snack-bar-danger');
        } else {
          this.notificationsService.showNotification(data.Message || 'There is some error!', 'snack-bar-danger');
        }
        this.isLoading = false;
      },
      error: (error: any) => {
        this.notificationsService.showNotification('Please fill the required fields!', 'snack-bar-danger');
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  displayAppointment = (appointment: any): string => {
    if (!appointment) {
      return '';
    }

    if (typeof appointment === 'string') {
      return appointment;
    }

    const token = appointment.tokenNumber ? `Token ${appointment.tokenNumber}` : `Appointment #${appointment.id}`;
    const patientName = appointment.patient?.name ? ` - ${appointment.patient.name}` : '';
    return `${token}${patientName}`;
  };

  onAppointmentSelected(appointment: any): void {
    if (!appointment) {
      return;
    }

    const index = this.appointments.findIndex((item: any) => item.id === appointment.id);
    if (index >= 0) {
      this.selectAppointment(index);
      return;
    }

    this.selectedAppointment = appointment;
    this.selectedIndex = 0;
    this.appointmentSearchCtrl.setValue(appointment, { emitEvent: false });
    this.loadTriageForAppointment(appointment.id);
  }

  onAppointmentInputCleared(event: Event): void {
    const value = (event.target as HTMLInputElement)?.value?.trim() ?? '';
    if (value.length > 0) {
      return;
    }

    this.appointmentSearchCtrl.setValue('', { emitEvent: false });
  }

  closeDialog(): void {
    if (this.data) {
      this.dialog.closeAll();
      return;
    }

    const canGoBack = window.history.length > 1;
    if (canGoBack) {
      window.history.back();
    } else {
      this.router.navigate(['/triage']);
    }
  }

  getPatientName(): string {
    return this.selectedAppointment?.patient?.name || 'Unknown Patient';
  }

  getPatientMrn(): string {
    return this.selectedAppointment?.patient?.mrn || `MRN-${this.selectedAppointment?.patientId || '-'}`;
  }

  getPatientAgeGender(): string {
    const patient = this.selectedAppointment?.patient;
    if (!patient) {
      return '-';
    }

    const age = patient.age ?? this.calculateAge(patient.dateOfBirth) ?? '-';
    const gender = patient.gender ? `${patient.gender}` : '-';
    return `${age} / ${gender}`;
  }

  getPatientPhone(): string {
    return this.selectedAppointment?.patient?.phoneNo || '-';
  }

  getDoctorName(): string {
    const doctor = this.selectedAppointment?.doctor;
    if (!doctor) {
      return '-';
    }
    return `${doctor.firstName || ''} ${doctor.lastName || ''}`.trim() || doctor.name || '-';
  }

  getDepartmentName(): string {
    return this.selectedAppointment?.department?.name || '-';
  }

  getReason(): string {
    return this.selectedAppointment?.reason || '-';
  }

  private calculateAge(dob: string | Date | null): number | null {
    if (!dob) {
      return null;
    }
    const birthDate = new Date(dob);
    const diff = Date.now() - birthDate.getTime();
    const ageDate = new Date(diff);
    return Math.abs(ageDate.getUTCFullYear() - 1970);
  }
}
