import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AppointmentService } from '../appointment.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { TriageService } from '../../triage/triage.service';
import { PatientProblemService } from '../../patientproblem/patientproblem.service';

@Component({
  selector: 'app-doctor-appointment-list',
  templateUrl: './doctor-appointment-list.component.html',
  styleUrls: ['./doctor-appointment-list.component.css'],
  standalone: false
})
export class DoctorAppointmentListComponent implements OnInit {
  // Data from API
  appointments: any[] = [];
  selectedAppointment: any = null;
  selectedIndex: number = 0;
  totalCount: number = 0;
  isLoading = false;

  // UI state
  activeTab: 'meds' | 'labs' | 'rad' = 'meds';
  checkedCount: number = 0;        // Number of patients checked (next button clicks)
  queueCount: number = 0;           // Remaining patients in queue

  // Form groups
  soapForm: FormGroup;
  appointmentFilterForm: FormGroup;

  // Local editable copies (to be sent to API)
  localProblems: any[] = [];
  localPrescriptions: any[] = [];
  localLabOrders: any[] = [];
  localRadiologyOrders: any[] = [];
  localVitals: any = { bpSystolic: 120, bpDiastolic: 80, pulse: 72, temperature: 98.6, spo2: 99 };

  // Modals visibility
  showProblemModal = false;
  showVitalsModal = false;

  newProblem = { name: '', onset: '', status: 'Active' };
  newVitals = { sys: 120, dia: 80, pulse: 72, temp: 98.6, spo2: 99 };

  // Temporary fields for adding orders
  newMed = { drug: '', dose: '', frequency: '', duration: '' };
  newLab = { test: '' };
  newRad = { scan: '', notes: '' };

  constructor(
    private fb: FormBuilder,
    private appointmentService: AppointmentService,
    private triageService: TriageService,
    private notificationsService: NotificationsService,
    private patientProblemService: PatientProblemService
  ) {
    this.soapForm = this.fb.group({
      subjective: [''],
      objective: [''],
      assessment: [''],
      plan: ['']
    });

    this.appointmentFilterForm = this.fb.group({
      fdate: [new Date()],
      tdate: [new Date()],
      statusId: [10]
    });
  }

  ngOnInit(): void {
    this.bindData();
  }

  async bindData(): Promise<void> {
    this.isLoading = true;
    const filter = {
      ...this.appointmentFilterForm.value,
      PagingData: { currentPage: 0, take: 50 }
    };

    (await this.appointmentService.getAllAppointmentByDoctor(filter)).subscribe({
      next: (data: any) => {
        this.appointments = data.item1;
        this.totalCount = data.item2;
        this.queueCount = this.appointments.length;

        if (this.appointments.length > 0) {
          this.selectAppointment(0);
        }
        this.isLoading = false;
      },
      error: (err: any) => {
        console.error(err);
        this.isLoading = false;
      }
    });
  }

  selectAppointment(index: number): void {
    this.selectedIndex = index;
    this.selectedAppointment = this.appointments[index];

    // Load problems from API
    this.localProblems = this.selectedAppointment.problems ? [...this.selectedAppointment.problems] : [];

    // Load prescriptions (medications)
    this.localPrescriptions = this.selectedAppointment.prescriptions ? [...this.selectedAppointment.prescriptions] : [];

    // Load lab orders
    this.localLabOrders = this.selectedAppointment.labOrders ? [...this.selectedAppointment.labOrders] : [];

    // Load radiology orders
    this.localRadiologyOrders = this.selectedAppointment.radiologyOrders ? [...this.selectedAppointment.radiologyOrders] : [];

    // Load latest triage (vitals)
    if (this.selectedAppointment.triages && this.selectedAppointment.triages.length > 0) {
      const triage = this.selectedAppointment.triages[0];
      this.localVitals = {
        bpSystolic: triage.systolicBp || 120,
        bpDiastolic: triage.diastolicBp || 80,
        pulse: triage.pulse || 72,
        temperature: triage.temperature || 98.6,
        spo2: triage.spo2 || 99
      };
    }

    // Load consultations (SOAP)
    if (this.selectedAppointment.consultations && this.selectedAppointment.consultations.length > 0) {
      const consultation = this.selectedAppointment.consultations[0];
      this.soapForm.patchValue({
        subjective: consultation.subjective || '',
        objective: consultation.objective || '',
        assessment: consultation.assessment || '',
        plan: consultation.plan || ''
      });
    } else {
      this.soapForm.reset();
    }
  }

  // Navigation
  nextPatient(): void {
    if (this.selectedIndex < this.appointments.length - 1) {
      this.selectAppointment(this.selectedIndex + 1);
      this.checkedCount++;
      this.queueCount--;
    } else {
      alert('No more patients in the queue.');
    }
  }

  prevPatient(): void {
    if (this.selectedIndex > 0) {
      this.selectAppointment(this.selectedIndex - 1);
      if (this.checkedCount > 0) this.checkedCount--;
      this.queueCount++;
    }
  }

  // Finish / Save consultation
  finishConsultation(): void {
    const consultationData = {
      appointmentId: this.selectedAppointment.id,
      subjective: this.soapForm.value.subjective,
      objective: this.soapForm.value.objective,
      assessment: this.soapForm.value.assessment,
      plan: this.soapForm.value.plan,
      problems: this.localProblems,
      prescriptions: this.localPrescriptions,
      labOrders: this.localLabOrders,
      radiologyOrders: this.localRadiologyOrders,
      vitals: this.localVitals,
      statusId : 3
    };

    this.appointmentService.saveConsultation(consultationData).subscribe({
      next: () => {
        alert('Consultation saved successfully!');
        // Optionally move to next patient
        this.nextPatient();
      },
      error: (err) => console.error('Save failed', err)
    });
  }

  // --- Problem Management ---
  async addProblem(): Promise<void> {
    if (!this.newProblem.name || !this.selectedAppointment?.id) return;

    this.isLoading = true;
    const payload = {
      id: 0,
      appointmentId: this.selectedAppointment.id,
      problem: this.newProblem.name,
      statusId: this.getStatusId(this.newProblem.status)
    };

    (await this.patientProblemService.savePatientProblem(payload)).subscribe({
      next: (data: any) => {
        if (data?.Status === 200 || data?.status === 200 || typeof data === 'number') {
          const problem = {
            id: data?.Data ?? data?.id ?? 0,
            name: this.newProblem.name,
            onset: this.newProblem.onset,
            status: this.newProblem.status,
            isActive: this.newProblem.status === 'Active'
          };
          this.localProblems.push(problem);
          this.newProblem = { name: '', onset: '', status: 'Active' };
          this.showProblemModal = false;
          this.notificationsService.showNotification(data?.Message || 'Problem saved successfully!', 'snack-bar-success');
        } else {
          this.notificationsService.showNotification(data?.Message || 'Unable to save problem.', 'snack-bar-danger');
        }
        this.isLoading = false;
      },
      error: () => {
        this.notificationsService.showNotification('Unable to save problem.', 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }

  private getStatusId(status: string): number {
    const key = (status || '').toLowerCase();
    if (key === 'managed') return 2;
    if (key === 'resolved') return 3;
    return 1;
  }

  removeProblem(index: number): void {
    this.localProblems.splice(index, 1);
  }

  // --- Medication Management ---
  addMedication(): void {
    if (!this.newMed.drug) return;
    const med = {
      id: 0,
      drugName: this.newMed.drug,
      dose: this.newMed.dose,
      frequency: this.newMed.frequency,
      duration: this.newMed.duration
    };
    this.localPrescriptions.push(med);
    this.newMed = { drug: '', dose: '', frequency: '', duration: '' };
  }

  removeMedication(index: number): void {
    this.localPrescriptions.splice(index, 1);
  }

  // --- Lab Order Management ---
  addLabOrder(): void {
    if (!this.newLab.test) return;
    const lab = {
      id: 0,
      testName: this.newLab.test
    };
    this.localLabOrders.push(lab);
    this.newLab = { test: '' };
  }

  removeLabOrder(index: number): void {
    this.localLabOrders.splice(index, 1);
  }

  // --- Radiology Order Management ---
  addRadiologyOrder(): void {
    if (!this.newRad.scan) return;
    const rad = {
      id: 0,
      scanType: this.newRad.scan,
      clinicalNotes: this.newRad.notes
    };
    this.localRadiologyOrders.push(rad);
    this.newRad = { scan: '', notes: '' };
  }

  removeRadiologyOrder(index: number): void {
    this.localRadiologyOrders.splice(index, 1);
  }

  // --- Vitals Management ---
  async updateVitals(): Promise<void> {
    if (!this.selectedAppointment?.id) {
      this.notificationsService.showNotification('Appointment not found.', 'snack-bar-danger');
      return;
    }

    this.isLoading = true;
    const latestTriage = this.getLatestTriage();
    const payload = {
      id: latestTriage?.id ?? 0,
      appointmentId: this.selectedAppointment.id,
      patientId: latestTriage?.patientId ?? this.selectedAppointment?.patientId ?? '00000000-0000-0000-0000-000000000000',
      nurseId: latestTriage?.nurseId ?? null,
      temperature: Number(this.newVitals.temp) || null,
      pulse: Number(this.newVitals.pulse) || null,
      systolicBp: Number(this.newVitals.sys) || null,
      diastolicBp: Number(this.newVitals.dia) || null,
      spo2: Number(this.newVitals.spo2) || null,
      weight: latestTriage?.weight ?? null,
      heightFeet: latestTriage?.heightFeet ?? null,
      heightInches: latestTriage?.heightInches ?? null,
      heightCm: latestTriage?.heightCm ?? null,
      bmi: latestTriage?.bmi ?? null,
      bloodSugar: latestTriage?.bloodSugar ?? null,
      sugarTypeId: latestTriage?.sugarTypeId ?? 1,
      triagePriorityId: latestTriage?.triagePriorityId ?? 1,
      chiefComplaint: latestTriage?.chiefComplaint ?? '',
      allergies: latestTriage?.allergies ?? '',
      medications: latestTriage?.medications ?? '',
      notes: latestTriage?.notes ?? '',
      triageScore: latestTriage?.triageScore ?? 0,
      triageCategoryId: latestTriage?.triageCategoryId ?? 0,
      takenAt: latestTriage?.takenAt ?? null
    };

    (await this.triageService.saveTriage(payload)).subscribe({
      next: (data: { Status: number; Message?: string }) => {
        if (data?.Status === 200) {
          this.localVitals = {
            bpSystolic: payload.systolicBp || 0,
            bpDiastolic: payload.diastolicBp || 0,
            pulse: payload.pulse || 0,
            temperature: payload.temperature || 0,
            spo2: payload.spo2 || 0
          };
          this.showVitalsModal = false;
          this.notificationsService.showNotification(data.Message || 'Vitals updated successfully!', 'snack-bar-success');
          this.bindData();
        } else {
          this.notificationsService.showNotification(data?.Message || 'Unable to update vitals.', 'snack-bar-danger');
        }
        this.isLoading = false;
      },
      error: () => {
        this.notificationsService.showNotification('Please fill the required fields!', 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }

  openVitalsModal(): void {
    this.newVitals = {
      sys: this.localVitals.bpSystolic,
      dia: this.localVitals.bpDiastolic,
      pulse: this.localVitals.pulse,
      temp: this.localVitals.temperature,
      spo2: this.localVitals.spo2
    };
    this.showVitalsModal = true;
  }

  // Helper for patient display
  getPatientName(): string {
    return this.selectedAppointment?.patient?.name || 'Unknown';
  }

  getPatientMRN(): string {
    return this.selectedAppointment?.patient?.mrn || 'N/A';
  }

  getPatientAgeGender(): string {
    const p = this.selectedAppointment?.patient;
    if (!p) return '';
    return `${p.age || '?'} ${p.gender === 'male' ? 'M' : 'F'}`;
  }

  getReason(): string {
    return this.selectedAppointment?.reason || 'No reason';
  }

  getLatestTriage(): any {
    return this.selectedAppointment?.triages?.[0] || null;
  }
}
