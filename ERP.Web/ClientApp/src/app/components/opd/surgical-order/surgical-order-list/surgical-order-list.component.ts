import { Component, Injectable, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { DateAdapter, MAT_DATE_FORMATS, NativeDateAdapter } from '@angular/material/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { SurgicalOrderService } from '../surgical-order.service';
import { ServiceService } from '../../service/service.service';
import { DoctorService } from '../../doctor/doctor.service';

@Injectable()
class SurgicalOrderDateAdapter extends NativeDateAdapter {
  override format(date: Date, displayFormat: Object): string {
    if (!this.isValid(date)) {
      return '';
    }

    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;
  }
}

const SURGICAL_ORDER_DATE_FORMATS = {
  parse: { dateInput: 'input' },
  display: {
    dateInput: 'input',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY',
  },
};

@Component({
  selector: 'app-surgical-order-list',
  templateUrl: './surgical-order-list.component.html',
  styleUrls: ['./surgical-order-list.component.css'],
  standalone: false,
  providers: [
    { provide: DateAdapter, useClass: SurgicalOrderDateAdapter },
    { provide: MAT_DATE_FORMATS, useValue: SURGICAL_ORDER_DATE_FORMATS },
  ]
})
export class SurgicalOrderListComponent implements OnInit, OnChanges {
  @Input() appointmentId: number | null = null;
  @Input() embeddedMode = false;
  @Input() reloadToken = 0;

  dataSource = new MatTableDataSource<any>([]);
  form!: FormGroup;
  displayedColumns: string[] = [];
  isLoading = false;
  currentPage = 0;
  pageSize = 10;
  totalRecords = 0;
  surgicalServices: any[] = [];
  doctors: any[] = [];

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private fb: FormBuilder,
    private constantService: ConstantService,
    private notifications: NotificationsService,
    private surgicalOrderService: SurgicalOrderService,
    private serviceService: ServiceService,
    private doctorService: DoctorService
  ) { }

  ngOnInit(): void {
    this.pageSize = this.embeddedMode
      ? 500
      : this.constantService.defaultItemPerPage;
    this.displayedColumns = this.embeddedMode
      ? ['service', 'surgeon', 'scheduledDateTime', 'status', 'notes', 'actions']
      : ['bookingNumber', 'patientName', 'service', 'surgeon', 'scheduledDateTime', 'status', 'actions'];

    if (!this.embeddedMode) {
      this.form = this.fb.group({
        surgeonId: [null],
        serviceId: [null],
        statusId: [null],
        name: [''],
        tokenNo: [''],
        fDate: [this.getDefaultFromDate()],
        tDate: [this.getDefaultToDate()]
      });
      this.loadLookups();
    }

    this.bindData();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['appointmentId'] || changes['reloadToken']) {
      this.currentPage = 0;
      this.bindData();
    }
  }

  loadLookups(): void {
    this.serviceService.getAllServices({ isSurgical: true, pagingData: { currentPage: 0, take: 500 } }).subscribe({
      next: (res: any) => this.surgicalServices = res?.item1 ?? [],
      error: () => this.surgicalServices = []
    });

    this.doctorService.getAllDoctors({ pagingData: { currentPage: 0, take: 500 } }).then(obs => {
      obs.subscribe({
        next: (res: any) => this.doctors = res?.item1 ?? [],
        error: () => this.doctors = []
      });
    });
  }

  filterData(): void {
    this.currentPage = 0;
    this.bindData();
  }

  bindData(): void {
    const appointmentId = this.appointmentId ?? null;
    let filter: any;

    if (this.embeddedMode && appointmentId) {
      filter = {
        appointmentId,
        pagingData: {
          currentPage: 0,
          take: 500
        }
      };
    } else {
      if (!this.form) {
        return;
      }

      const raw = this.form.value;
      filter = {
        surgeonId: raw.surgeonId || null,
        serviceId: raw.serviceId || null,
        statusId: raw.statusId || null,
        name: raw.name || '',
        tokenNo: raw.tokenNo || '',
        appointmentId: null,
        pagingData: {
          currentPage: this.currentPage,
          take: this.pageSize
        },
        fDate: this.constantService.formatDate(raw.fDate),
        tDate: this.constantService.formatDate(raw.tDate)
      };
    }

    this.isLoading = true;
    this.surgicalOrderService.getAllSurgicalOrders(filter).subscribe({
      next: (data: any) => {
        if (data?.Status && data.Status !== 200) {
          this.dataSource.data = [];
          this.totalRecords = 0;
          this.isLoading = false;
          this.notifications.showNotification(data?.Message || 'Error loading surgical orders', 'snack-bar-danger');
          return;
        }

        const rows = data?.item1 ?? data?.Item1 ?? [];
        this.dataSource.data = Array.isArray(rows) ? rows : [];
        this.totalRecords = data?.item2 ?? data?.Item2 ?? this.dataSource.data.length;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.notifications.showNotification('Error loading surgical orders', 'snack-bar-danger');
      }
    });
  }

  pageChanged(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  deleteOrder(element: any): void {
    if (!confirm('Delete this surgical order?')) return;

    this.surgicalOrderService.deleteSurgicalOrder(element.id).subscribe({
      next: (res: any) => {
        if (res?.Status === 200) {
          this.notifications.showNotification('Surgical order deleted', 'snack-bar-success');
          this.bindData();
        } else {
          this.notifications.showNotification(res?.Message || 'Delete failed', 'snack-bar-danger');
        }
      },
      error: () => this.notifications.showNotification('Delete failed', 'snack-bar-danger')
    });
  }

  getDoctorName(doctor: any): string {
    if (!doctor) return '-';
    return `${doctor.firstName || ''} ${doctor.lastName || ''}`.trim() || '-';
  }

  getPatientName(element: any): string {
    return element?.appointment?.patient?.patientMaster?.name || '-';
  }

  private getDefaultFromDate(): Date {
    const date = new Date();
    date.setDate(date.getDate() - 5);
    date.setHours(0, 0, 0, 0);
    return date;
  }

  private getDefaultToDate(): Date {
    const date = new Date();
    date.setDate(date.getDate() + 5);
    date.setHours(0, 0, 0, 0);
    return date;
  }
}
