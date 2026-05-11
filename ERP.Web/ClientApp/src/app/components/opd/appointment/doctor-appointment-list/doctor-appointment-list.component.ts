import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AppointmentService } from '../appointment.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { TriageService } from '../../triage/triage.service';
import { PatientProblemService } from '../../patientproblem/patientproblem.service';
import { PrescriptionService } from '../../prescription/prescription.service';
import { LabOrderService } from '../../lab-order/lab-order.service';
import { LabOrderTypeService } from '../../lab-order-type/lab-order-type.service';
import { RadiologyTypeService } from '../../radiologytype/radiologytype.service';
import { RadiologyOrderService } from '../../radiologyorder/radiologyorder.service';

@Component({
  selector: 'app-doctor-appointment-list',
  templateUrl: './doctor-appointment-list.component.html',
  styleUrls: ['./doctor-appointment-list.component.css'],
  standalone: false
})
export class DoctorAppointmentListComponent implements OnInit {
  appointments: any[] = [];
  selectedAppointment: any = null;
  selectedIndex: number = 0;
  totalCount: number = 0;
  isLoading = false;

  activeTab: 'meds' | 'labs' | 'rad' = 'meds';
  checkedCount: number = 0;
  queueCount: number = 0;

  soapForm: FormGroup;
  appointmentFilterForm: FormGroup;

  localProblems: any[] = [];
  localPrescriptions: any[] = [];
  localLabOrders: any[] = [];
  localRadiologyOrders: any[] = [];
  labOrderTypes: any[] = [];
  radiologyTypes: any[] = [];
  localVitals: any = { bpSystolic: 120, bpDiastolic: 80, pulse: 72, temperature: 98.6, spo2: 99 };

  showProblemModal = false;
  showVitalsModal = false;

  newProblem = { problem: '', onset: '', status: { id: 200, title: 'Active' } };
  newVitals = { sys: 120, dia: 80, pulse: 72, temp: 98.6, spo2: 99 };

  newMed = { drug: '', dose: '', frequency: '', duration: '', instructions: '' };
  newLab = { test: '' };
  newRad = { scan: '', notes: '' };

  constructor(
    private fb: FormBuilder,
    private appointmentService: AppointmentService,
    private triageService: TriageService,
    private notificationsService: NotificationsService,
    private patientProblemService: PatientProblemService,
    private prescriptionService: PrescriptionService,
    private labOrderService: LabOrderService,
    private labOrderTypeService: LabOrderTypeService,
    private radiologyTypeService: RadiologyTypeService,
    private radiologyOrderService: RadiologyOrderService
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
    this.loadLabOrderTypes();
    this.loadRadiologyTypes();
  }

  loadLabOrderTypes(): void {
    const filter = { pagingData: { currentPage: 0, take: 200 } };
    this.labOrderTypeService.getAllLabOrderTypes(filter).subscribe({
      next: (res: any) => this.labOrderTypes = res?.item1 || [],
      error: () => this.labOrderTypes = []
    });
  }

  loadRadiologyTypes(): void {
    const filter = { pagingData: { currentPage: 0, take: 200 } };
    this.radiologyTypeService.getAllRadiologyTypes(filter).subscribe({
      next: (res: any) => this.radiologyTypes = res?.Data || [],
      error: () => this.radiologyTypes = []
    });
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
      error: (err) => {
        console.error(err);
        this.isLoading = false;
      }
    });
  }

  selectAppointment(index: number): void {
    this.selectedIndex = index;
    this.selectedAppointment = this.appointments[index];

    this.localProblems = this.selectedAppointment.problems ? [...this.selectedAppointment.problems] : [];
    this.localPrescriptions = this.selectedAppointment.prescriptions ? [...this.selectedAppointment.prescriptions] : [];
    this.localLabOrders = this.selectedAppointment.labOrders ? [...this.selectedAppointment.labOrders] : [];
    this.localRadiologyOrders = this.selectedAppointment.radiologyOrders ? [...this.selectedAppointment.radiologyOrders] : [];

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
        this.nextPatient();
      },
      error: (err) => console.error('Save failed', err)
    });
  }

  async addProblem(): Promise<void> {
    if (!this.newProblem.problem || !this.selectedAppointment?.id) return;

    this.isLoading = true;
    const statusId = Number(this.newProblem.status.id);
    const payload = {
      id: 0,
      appointmentId: this.selectedAppointment.id,
      problem: this.newProblem.problem,
      onset: this.newProblem.onset,
      statusId: statusId,
    };

    (await this.patientProblemService.savePatientProblem(payload)).subscribe({
      next: (data: any) => {
        if (data?.Status === 200 || data?.status === 200 || typeof data === 'number') {
          const statusTitle = this.getStatusTitle(statusId);
          const problem = {
            id: data?.Data ?? data?.id ?? 0,
            problem: this.newProblem.problem,
            onset: this.newProblem.onset,
            status: { id: statusId, title: statusTitle },
            isActive: statusId === 200
          };
          this.localProblems.push(problem);
          this.newProblem = { problem: '', onset: '', status: { id: 200, title: 'Active' } };
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

  getStatusTitle(statusId: number | string): string {
    const id = Number(statusId);
    switch (id) {
      case 200: return 'Active';
      case 201: return 'Managed';
      case 202: return 'Resolved';
      default: return 'Active';
    }
  }

  async removeProblem(index: number): Promise<void> {
    const problem = this.localProblems[index];
    if (problem?.id == null) {
      this.localProblems.splice(index, 1);
      return;
    }

    this.isLoading = true;
    (await this.patientProblemService.deletePatientProblem(problem.id)).subscribe({
      next: (data: any) => {
        if (data === true) {
          this.localProblems.splice(index, 1);
          this.notificationsService.showNotification(data?.Message || 'Problem deleted successfully!', 'snack-bar-success');
        } else {
          this.notificationsService.showNotification(data?.Message || 'Unable to delete problem.', 'snack-bar-danger');
        }
        this.isLoading = false;
      },
      error: () => {
        this.notificationsService.showNotification('Unable to delete problem.', 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }

  async addMedication(): Promise<void> {
    if (!this.newMed.drug || !this.selectedAppointment?.id) {
      return;
    }

    this.isLoading = true;
    const payload = {
      id: 0,
      appointmentId: this.selectedAppointment.id,
      drugName: this.newMed.drug,
      dosage: this.newMed.dose,
      frequency: this.newMed.frequency,
      duration: this.newMed.duration,
      instructions: this.newMed.instructions || ''
    };

    (await this.prescriptionService.savePrescription(payload)).subscribe({
      next: (data: any) => {
        if (data?.Status === 200 || data?.status === 200 || typeof data === 'number') {
          const med = {
            id: data?.Data ?? data?.id ?? 0,
            drugName: payload.drugName,
            dose: payload.dosage,
            frequency: payload.frequency,
            duration: payload.duration
          };
          this.localPrescriptions.push(med);
          this.newMed = { drug: '', dose: '', frequency: '', duration: '', instructions: '' };
          this.notificationsService.showNotification(data?.Message || 'Medication saved successfully!', 'snack-bar-success');
        } else {
          this.notificationsService.showNotification(data?.Message || 'Unable to save medication.', 'snack-bar-danger');
        }
        this.isLoading = false;
      },
      error: () => {
        this.notificationsService.showNotification('Unable to save medication.', 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }

  async removeMedication(index: number): Promise<void> {
    const prescription = this.localPrescriptions[index];
    if (prescription?.id == null) {
      this.localPrescriptions.splice(index, 1);
      return;
    }

    this.isLoading = true;
    (await this.prescriptionService.deletePrescription(prescription.id)).subscribe({
      next: (data: any) => {
        if (data === true) {
          this.localPrescriptions.splice(index, 1);
          this.notificationsService.showNotification(data?.Message || 'Medication deleted successfully!', 'snack-bar-success');
        } else {
          this.notificationsService.showNotification(data?.Message || 'Unable to delete medication.', 'snack-bar-danger');
        }
        this.isLoading = false;
      },
      error: () => {
        this.notificationsService.showNotification('Unable to delete medication.', 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }

  async addLabOrder(): Promise<void> {
    if (!this.newLab.test || !this.selectedAppointment?.id) return;

    this.isLoading = true;
    const selectedTypeId = Number(this.newLab.test);
    if (!selectedTypeId) {
      this.notificationsService.showNotification('Please select a valid lab test.', 'snack-bar-danger');
      this.isLoading = false;
      return;
    }

    const payload = {
      id: 0,
      appointmentId: this.selectedAppointment.id,
      labOrderTypeId: selectedTypeId,
      statusId: 1
    };

    (await this.labOrderService.saveLabOrder(payload)).subscribe({
      next: (data: any) => {
        if (data?.Status === 200 || data?.status === 200 || typeof data === 'number') {
          const label = this.getLabOrderTypeName(selectedTypeId);
          this.localLabOrders.push({ id: data?.Data ?? 0, labOrderTypeId: selectedTypeId, testName: label });
          this.newLab = { test: '' };
          this.notificationsService.showNotification(data?.Message || 'Lab order saved successfully!', 'snack-bar-success');
        } else {
          this.notificationsService.showNotification(data?.Message || 'Unable to save lab order.', 'snack-bar-danger');
        }
        this.isLoading = false;
      },
      error: () => {
        this.notificationsService.showNotification('Unable to save lab order.', 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }

  getLabOrderTypeName(id: number): string {
    const option = this.labOrderTypes.find(x => Number(x.id) === Number(id));
    return option?.name || 'Selected Test';
  }

  async removeLabOrder(index: number): Promise<void> {
    const labOrder = this.localLabOrders[index];
    if (!labOrder?.id) {
      this.localLabOrders.splice(index, 1);
      return;
    }

    this.isLoading = true;
    (await this.labOrderService.deleteLabOrder(labOrder.id)).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res.Status === 200 || res === true) {
          this.localLabOrders.splice(index, 1);
          this.notificationsService.showNotification(res?.Message || 'Lab order deleted successfully!', 'snack-bar-success');
        } else {
          this.notificationsService.showNotification(res?.Message || 'Unable to delete lab order.', 'snack-bar-danger');
        }
      },
      error: () => {
        this.isLoading = false;
        this.notificationsService.showNotification('Error deleting lab order.', 'snack-bar-danger');
      }
    });
  }

  addRadiologyOrder(): void {
    if (!this.newRad.scan) return;

    const selectedRadType = this.radiologyTypes.find(r => r.name === this.newRad.scan);
    if (!selectedRadType) {
      this.notificationsService.showNotification('Invalid radiology type selected.', 'snack-bar-danger');
      return;
    }

    const payload = {
      appointmentId: this.selectedAppointment.id,
      radiologyTypeId: selectedRadType.id,
      clinicalNotes: this.newRad.notes || '',
      statusId: 1
    };

    this.isLoading = true;
    this.radiologyOrderService.saveRadiologyOrder(payload).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res.Status === 200) {
          const radId = res.Data || 0;
          this.localRadiologyOrders.push({
            id: radId,
            scanType: this.newRad.scan,
            clinicalNotes: this.newRad.notes
          });
          this.newRad = { scan: '', notes: '' };
          this.notificationsService.showNotification('Imaging order saved!', 'snack-bar-success');
        } else {
          this.notificationsService.showNotification(res.Message || 'Error saving imaging order.', 'snack-bar-danger');
        }
      },
      error: (err) => {
        this.isLoading = false;
        this.notificationsService.showNotification('Error saving imaging order.', 'snack-bar-danger');
      }
    });
  }

  removeRadiologyOrder(index: number): void {
    this.localRadiologyOrders.splice(index, 1);
  }

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
