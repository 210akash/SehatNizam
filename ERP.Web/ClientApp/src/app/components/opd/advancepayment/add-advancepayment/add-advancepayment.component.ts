import { Component, Inject, Optional, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { AdvancePaymentListComponent } from '../advancepayment-list/advancepayment-list.component';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { AdvancePaymentService } from '../advancepayment.service';
import { PaymentModeService } from '../../../paymentmode/paymentmode.service';
import { AppointmentService } from '../../appointment/appointment.service';
import { Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, finalize, map, startWith, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-add-advancepayment',
  templateUrl: './add-advancepayment.component.html',
  styleUrl: './add-advancepayment.component.css',
  standalone: false
})
export class AddAdvancePaymentComponent {
  admissionServiceForm!: FormGroup;
  isLoading = false;
  paymentModesList: any;
  selectedAppointment: any = null;
  appointmentSearchCtrl = new FormControl<string | any>('');
  filteredAppointments$!: Observable<any[]>;
  appointmentLoading = false;

  constructor( private admissionServiceService: AdvancePaymentService, 
    private formBuilder: FormBuilder, 
    private dialog: MatDialog, 
    private notificationsService: NotificationsService, 
     private paymentModeService: PaymentModeService,
    private constantService: ConstantService, 
    private appointmentService: AppointmentService,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: { element: any } | null) { }
  @ViewChild(AdvancePaymentListComponent) advancepaymentListComponent!: AdvancePaymentListComponent;

  ngOnInit(): void {
    const element = this.data?.element ?? null;
    const elementId = element?.id ?? 0;
    const appointmentId = element?.appointmentId ?? null;
    
    this.admissionServiceForm = this.formBuilder.group({
      id: [elementId],
      appoinmentno : [''],
      appointmentId: [appointmentId, Validators.required],
      amount: [0, [Validators.required, Validators.min(0.01)]],
      paymentModeId: [5, Validators.required],
      paymentStatusId: [1]
    });

    if (this.data?.element?.appointment) {
      this.selectedAppointment = this.data.element.appointment;
      this.appointmentSearchCtrl.setValue(this.selectedAppointment, { emitEvent: false });
    }
   
    this.getAllPaymentModes();
    this.setupAppointmentAutocomplete();
  }

  getAllPaymentModes() {
    this.paymentModeService.getAllPaymentModes({})
      .subscribe((d: any) => this.paymentModesList = d?.item1 ?? []);
  }

  SaveData() {
    if (this.admissionServiceForm.invalid) {
      this.constantService.markFormGroupTouched(this.admissionServiceForm);
      return;
    }

    this.isLoading = true;
    const payload = {
      ...this.admissionServiceForm.value,
      appointmentId: this.admissionServiceForm.get('appointmentId')?.value
    };

    this.admissionServiceService.saveAdvancePayment(payload).subscribe({
      next: (data) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.admissionServiceForm.reset();
          this.dialog.closeAll();
        }
        else
          this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        this.isLoading = false;
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
          : String(value?.id ?? '').trim();

        if (!term) {
          return of([]);
        }

        this.appointmentLoading = true;
        return this.appointmentService.getAppointmentByToken(term,0).pipe(
          map((data: any) => data?.item1 ?? data ?? []),
          finalize(() => (this.appointmentLoading = false))
        );
      })
    );
  }

  displayAppointment = (appointment: any): string => {
    if (!appointment) {
      return '';
    }

    if (typeof appointment === 'string') {
      return appointment;
    }

    const token = `Token # ${appointment.tokenNumber}`;
    const patientName = appointment.patient?.patientMaster?.name ? ` - ${appointment.patient?.patientMaster?.name}` : '';
    return `${token}${patientName}`;
  };

  onAppointmentSelected(appointment: any): void {
    if (!appointment) {
      return;
    }

    this.selectedAppointment = appointment;
    this.appointmentSearchCtrl.setValue(appointment, { emitEvent: false });
    this.admissionServiceForm.get('appointmentId')?.setValue(appointment.id);
  }

  onAppointmentInputCleared(event: Event): void {
    const value = (event.target as HTMLInputElement)?.value?.trim() ?? '';
    if (value.length > 0) {
      return;
    }

    this.appointmentSearchCtrl.setValue('', { emitEvent: false });
    this.admissionServiceForm.get('appointmentId')?.setValue(null);
  }

  lookupAppointment(): void {
    const value = this.appointmentSearchCtrl.value;

    if (!value) {
      return;
    }

    if (typeof value === 'object' && value?.id) {
      this.onAppointmentSelected(value);
      return;
    }

    const term = String(value).trim();
    if (!term) {
      return;
    }

    this.appointmentLoading = true;
    this.appointmentService.getAppointmentByToken(term, 5)
      .pipe(finalize(() => (this.appointmentLoading = false)))
      .subscribe({
        next: (data: any) => {
          const appointments = data?.item1 ?? data ?? [];
          const appointment = Array.isArray(appointments) ? appointments[0] : appointments;

          if (!appointment) {
            this.notificationsService.showNotification('No appointment found for the entered token.', 'snack-bar-danger');
            return;
          }

          this.onAppointmentSelected(appointment);
        },
        error: (error: any) => {
          console.log(error);
          this.notificationsService.showNotification('Unable to search appointment by token.', 'snack-bar-danger');
        }
      });
  }
}