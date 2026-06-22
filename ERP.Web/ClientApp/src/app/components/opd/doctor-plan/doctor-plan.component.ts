import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { CalendarOptions, EventClickArg, EventContentArg, EventInput } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import { forkJoin } from 'rxjs';
import { ConstantService } from '../../../Service/constant.service';
import { NotificationsService } from '../../../Service/notification.service';
import { AppointmentService } from '../appointment/appointment.service';
import { DoctorService } from '../doctor/doctor.service';
import { SurgicalOrderService } from '../surgical-order/surgical-order.service';
import { DoctorPlanDetailData, DoctorPlanDetailDialogComponent } from './doctor-plan-detail-dialog.component';
import { DoctorPlanListDialogComponent } from './doctor-plan-list-dialog.component';

interface ScheduleEventProps {
  type: 'appointment' | 'surgical';
  patientName: string;
  bookingNumber: string;
  serviceOrDept: string;
  status: string;
  notes: string;
  isSummary?: boolean;
  count?: number;
  items?: DoctorPlanDetailData[];
}

@Component({
  selector: 'app-doctor-plan',
  templateUrl: './doctor-plan.component.html',
  styleUrls: ['./doctor-plan.component.css'],
  standalone: false
})
export class DoctorPlanComponent implements OnInit {
  form!: FormGroup;
  doctors: any[] = [];
  isLoading = false;
  appointmentCount = 0;
  surgeryCount = 0;
  visibleRange: { start: Date; end: Date } | null = null;
  currentView = 'dayGridMonth';

  calendarOptions: CalendarOptions = {
    plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
    initialView: 'dayGridMonth',
    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'dayGridMonth,timeGridWeek,timeGridDay'
    },
    height: 'auto',
    expandRows: true,
    slotMinTime: '08:00:00',
    slotMaxTime: '21:00:00',
    allDaySlot: false,
    nowIndicator: true,
    displayEventTime: false,
    dayMaxEvents: 4,
    eventDisplay: 'block',
    events: [],
    eventContent: (arg) => this.renderEventContent(arg),
    eventDidMount: (info) => this.decorateEvent(info),
    eventClick: (arg) => this.onEventClick(arg),
    datesSet: (info) => {
      this.visibleRange = { start: info.start, end: info.end };
      this.currentView = info.view.type;
      this.loadSchedule();
    }
  };

  constructor(
    private fb: FormBuilder,
    private dialog: MatDialog,
    private constantService: ConstantService,
    private notifications: NotificationsService,
    private appointmentService: AppointmentService,
    private surgicalOrderService: SurgicalOrderService,
    private doctorService: DoctorService
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      doctorId: [null, Validators.required]
    });

    this.form.get('doctorId')?.valueChanges.subscribe(() => {
      this.loadSchedule();
    });

    this.loadDoctors();
  }

  loadDoctors(): void {
    this.doctorService.getAllDoctors({ pagingData: { currentPage: 0, take: 500 } }).then(obs => {
      obs.subscribe({
        next: (res: any) => {
          this.doctors = res?.item1 ?? [];
          if (this.doctors.length === 1) {
            this.form.patchValue({ doctorId: this.doctors[0].id });
          }
        },
        error: () => this.doctors = []
      });
    });
  }

  loadSchedule(): void {
    const doctorId = this.form?.value?.doctorId;
    if (!doctorId || !this.visibleRange) {
      this.calendarOptions = { ...this.calendarOptions, events: [] };
      this.appointmentCount = 0;
      this.surgeryCount = 0;
      return;
    }

    const filterBase = {
      fDate: this.constantService.formatDate(this.visibleRange.start),
      tDate: this.constantService.formatDate(this.addDays(this.visibleRange.end, -1)),
      pagingData: { currentPage: 0, take: 1000 }
    };

    this.isLoading = true;
    forkJoin({
      appointments: this.appointmentService.getAllAppointments({
        ...filterBase,
        doctorId,
        bookingFormType: 1,
        patientName: '',
        tokenNo: '',
        mRN: ''
      }),
      surgicalOrders: this.surgicalOrderService.getAllSurgicalOrders({
        ...filterBase,
        surgeonId: doctorId,
        appointmentId: null,
        serviceId: null,
        statusId: null,
        name: '',
        tokenNo: ''
      })
    }).subscribe({
      next: (result) => {
        const appointments = result.appointments?.item1 ?? result.appointments?.Item1 ?? [];
        const surgicalOrders = result.surgicalOrders?.item1 ?? result.surgicalOrders?.Item1 ?? [];
        this.appointmentCount = appointments.length;
        this.surgeryCount = surgicalOrders.length;
        this.calendarOptions = {
          ...this.calendarOptions,
          events: this.buildEvents(appointments, surgicalOrders)
        };
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.notifications.showNotification('Error loading doctor schedule', 'snack-bar-danger');
      }
    });
  }

  onEventClick(arg: EventClickArg): void {
    arg.jsEvent.preventDefault();
    const props = arg.event.extendedProps as unknown as ScheduleEventProps;

    if (props.isSummary && props.items?.length) {
      if (props.items.length === 1) {
        this.openDetail(props.items[0]);
        return;
      }

      const listRef = this.dialog.open(DoctorPlanListDialogComponent, {
        data: { type: props.type, items: props.items },
        panelClass: 'doctor-plan-list-panel',
        maxWidth: '95vw',
        autoFocus: false
      });

      listRef.afterClosed().subscribe((item: DoctorPlanDetailData | undefined) => {
        if (item) {
          this.openDetail(item);
        }
      });
      return;
    }

    this.openDetail({
      type: props.type,
      title: props.type === 'appointment' ? 'Appointment' : 'Surgical Order',
      start: arg.event.start ?? new Date(),
      end: arg.event.end ?? arg.event.start ?? new Date(),
      patientName: props.patientName,
      bookingNumber: props.bookingNumber,
      serviceOrDept: props.serviceOrDept,
      status: props.status,
      notes: props.notes
    });
  }

  getDoctorName(doctor: any): string {
    if (!doctor) return '-';
    return `${doctor.firstName || ''} ${doctor.lastName || ''}`.trim() || '-';
  }

  getSelectedDoctorName(): string {
    const id = this.form?.value?.doctorId;
    const doctor = this.doctors.find(d => d.id === id);
    return doctor ? this.getDoctorName(doctor) : '';
  }

  private openDetail(data: DoctorPlanDetailData): void {
    this.dialog.open(DoctorPlanDetailDialogComponent, {
      data,
      panelClass: 'doctor-plan-detail-panel',
      maxWidth: '95vw',
      autoFocus: false
    });
  }

  private renderEventContent(arg: EventContentArg) {
    const props = arg.event.extendedProps as Record<string, unknown>;
    const type = props['type'] as string;
    const isMonth = arg.view.type === 'dayGridMonth';

    if (isMonth && props['isSummary']) {
      const count = props['count'] as number;
      const label = type === 'appointment' ? 'Appointment' : 'Surgery';
      return {
        html: `<span class="schedule-pill ${type}"><span class="pill-label">${label}</span><span class="pill-count">${count}</span></span>`
      };
    }

    if (isMonth) {
      return { html: `<span class="schedule-bullet ${type}" role="presentation"></span>` };
    }

    return { html: `<div class="schedule-time-marker ${type}"><span class="schedule-bullet ${type}"></span></div>` };
  }

  private decorateEvent(info: { el: HTMLElement; event: { extendedProps: Record<string, unknown>; start: Date | null } }): void {
    const props = info.event.extendedProps as unknown as ScheduleEventProps;

    if (props.isSummary) {
      const label = props.type === 'appointment' ? 'Appointments' : 'Surgeries';
      info.el.setAttribute('title', `${label}: ${props.count}`);
      info.el.classList.add('schedule-event', 'schedule-summary', props.type);
      return;
    }

    const start = info.event.start;
    const time = start
      ? start.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
      : '';
    const label = props.type === 'appointment' ? 'Appointment' : 'Surgery';
    info.el.setAttribute('title', `${label} · ${props.patientName} · ${time}`);
    info.el.classList.add('schedule-event', props.type);
  }

  private buildEvents(appointments: any[], surgicalOrders: any[]): EventInput[] {
    const appointmentEvents = appointments.map((a) => this.mapAppointmentEvent(a));
    const surgicalEvents = surgicalOrders.map((s) => this.mapSurgicalEvent(s));

    if (this.currentView === 'dayGridMonth') {
      return [
        ...this.buildMonthSummaryEvents(appointmentEvents, 'appointment'),
        ...this.buildMonthSummaryEvents(surgicalEvents, 'surgical')
      ];
    }

    return [...appointmentEvents, ...surgicalEvents];
  }

  private buildMonthSummaryEvents(events: EventInput[], type: 'appointment' | 'surgical'): EventInput[] {
    const grouped = new Map<string, DoctorPlanDetailData[]>();

    events.forEach((event) => {
      const start = event.start as Date;
      const dayKey = this.toDayKey(start);
      const props = event.extendedProps as ScheduleEventProps;
      const end = (event.end as Date) ?? start;
      const items = grouped.get(dayKey) ?? [];
      items.push({
        type,
        title: type === 'appointment' ? 'Appointment' : 'Surgical Order',
        start: new Date(start),
        end: new Date(end),
        patientName: props.patientName,
        bookingNumber: props.bookingNumber,
        serviceOrDept: props.serviceOrDept,
        status: props.status,
        notes: props.notes
      });
      grouped.set(dayKey, items);
    });

    return Array.from(grouped.entries()).map(([dayKey, items]) => ({
      id: `summary-${type}-${dayKey}`,
      title: ' ',
      start: dayKey,
      allDay: true,
      classNames: ['schedule-event', 'schedule-summary', type],
      extendedProps: {
        type,
        isSummary: true,
        count: items.length,
        items
      } as ScheduleEventProps
    }));
  }

  private mapAppointmentEvent(a: any): EventInput {
    const start = new Date(a.appointmentDate);
    const patientName = a.patient?.patientMaster?.name || 'Patient';
    return {
      id: `appt-${a.id}`,
      title: ' ',
      start,
      end: this.addMinutes(start, 30),
      classNames: ['schedule-event', 'appointment'],
      extendedProps: {
        type: 'appointment',
        patientName,
        bookingNumber: String(a.id),
        serviceOrDept: a.department?.name || '-',
        status: a.appointmentStatus?.title || '-',
        notes: a.reason || '-'
      } as ScheduleEventProps
    };
  }

  private mapSurgicalEvent(s: any): EventInput {
    const start = new Date(s.scheduledDateTime);
    const serviceName = s.service?.name || 'Surgery';
    const patientName = s.appointment?.patient?.patientMaster?.name || '-';
    return {
      id: `surg-${s.id}`,
      title: ' ',
      start,
      end: this.addMinutes(start, 60),
      classNames: ['schedule-event', 'surgical'],
      extendedProps: {
        type: 'surgical',
        patientName,
        bookingNumber: String(s.appointmentId || '-'),
        serviceOrDept: serviceName,
        status: s.status?.title || '-',
        notes: s.clinicalNotes || '-'
      } as ScheduleEventProps
    };
  }

  private toDayKey(date: Date): string {
    const y = date.getFullYear();
    const m = String(date.getMonth() + 1).padStart(2, '0');
    const d = String(date.getDate()).padStart(2, '0');
    return `${y}-${m}-${d}`;
  }

  private addMinutes(date: Date, minutes: number): Date {
    const result = new Date(date);
    result.setMinutes(result.getMinutes() + minutes);
    return result;
  }

  private addDays(date: Date, days: number): Date {
    const result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
  }
}
