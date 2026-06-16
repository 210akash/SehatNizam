import { Component, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, finalize, map, startWith, switchMap } from 'rxjs/operators';
import { AppointmentService } from '../../../opd/appointment/appointment.service';
import { DeleteBloodRequestComponent } from '../../blood-request/delete-blood-request/delete-blood-request.component';
import { BloodRequestLogComponent } from '../../blood-request/blood-request-log/blood-request-log.component';
import { BloodRequestService } from '../../blood-request/blood-request.service';
import { BloodGroupService } from '../../blood-group/blood-group.service';
import { ComponentTypeService } from '../../component-type/component-type.service';
import { ProcessTransfusionComponent } from '../process-transfusion/process-transfusion.component';

@Component({
    selector: 'app-transfusion-list',
    templateUrl: './transfusion-list.component.html',
    styleUrls: ['./transfusion-list.component.css'],
    standalone: false
})
export class TransfusionListComponent {
    filterForm!: FormGroup;
    appointmentSearchCtrl = new FormControl<string | any>('');
    filteredAppointments$!: Observable<any[]>;
    selectedAppointment: any = null;
    appointmentLoading = false;
    bloodGroupList: any[] = [];
    componentTypeList: any[] = [];
    currentPage = 0;
    take = 10;
    totalRows = 0;
    pageSizeOptions: number[] = [5, 10, 25, 100];
    displayedColumns: string[] = ['code', 'patientName', 'patientCNIC', 'bloodGroup', 'componentType', 'quantity', 'requestDate', 'status', 'actions'];
    dataSource: any;
    statusMap: { [key: number]: string } = {
        1: 'Pending',
        2: 'Cross Matched',
        3: 'Issued',
        4: 'Cancelled'
    };
    statusFilterOptions = [
        { value: 0, name: 'All' },
        { value: 1, name: 'Pending' },
        { value: 2, name: 'Cross Matched' },
        { value: 3, name: 'Issued' },
        { value: 4, name: 'Cancelled' }
    ];

    @ViewChild(MatPaginator) paginator!: MatPaginator;
    @ViewChild(MatSort) sort!: MatSort;

    constructor(
        private bloodRequestService: BloodRequestService,
        private bloodGroupService: BloodGroupService,
        private componentTypeService: ComponentTypeService,
        private appointmentService: AppointmentService,
        private dialog: MatDialog,
        private formBuilder: FormBuilder
    ) { }

    ngOnInit(): void {
        this.filterForm = this.formBuilder.group({
            patientName: [''],
            patientCNIC: [''],
            status: [0],
            bloodGroupMasterId: [null],
            bloodComponentTypeId: [null],
            appointmentId: [null]
        });
        this.setupAppointmentAutocomplete();
        this.loadLookups();
        this.bindData();
    }

    loadLookups(): void {
        this.bloodGroupService.getAll({ PagingData: { currentPage: 0, take: 1000 } }).subscribe((data: any) => {
            this.bloodGroupList = data.item1 || [];
        });
        this.componentTypeService.getAll({ PagingData: { currentPage: 0, take: 1000 } }).subscribe((data: any) => {
            this.componentTypeList = data.item1 || [];
        });
    }

    bindData(): void {
        const raw = this.filterForm.value;
        const request: any = {
            patientName: raw.patientName,
            patientCNIC: raw.patientCNIC,
            PagingData: { currentPage: this.currentPage, take: this.take }
        };

        if (raw.status) {
            request.status = raw.status;
        }
        if (raw.bloodGroupMasterId) {
            request.bloodGroupMasterId = raw.bloodGroupMasterId;
        }
        if (raw.bloodComponentTypeId) {
            request.bloodComponentTypeId = raw.bloodComponentTypeId;
        }
        if (raw.appointmentId) {
            request.appointmentId = raw.appointmentId;
        }

        this.bloodRequestService.getAll(request).subscribe({
            next: (data: any) => {
                this.dataSource = new MatTableDataSource(data.item1);
                this.totalRows = data.item2;
                this.dataSource.sort = this.sort;
                if (data.item1?.length > 0) {
                    setTimeout(() => {
                        this.paginator.pageIndex = this.currentPage;
                        this.paginator.length = this.totalRows;
                    });
                }
            }
        });
    }

    pageChanged(event: PageEvent): void {
        this.take = event.pageSize;
        this.currentPage = event.pageIndex;
        this.bindData();
    }

    filterData(): void {
        this.currentPage = 0;
        this.bindData();
    }

    onAppointmentSelected(appointment: any): void {
        if (!appointment?.id) return;
        this.selectedAppointment = appointment;
        this.appointmentSearchCtrl.setValue(appointment, { emitEvent: false });
        this.filterForm.patchValue({ appointmentId: appointment.id });
        this.filterData();
    }

    onAppointmentInputCleared(event: Event): void {
        const value = (event.target as HTMLInputElement)?.value?.trim() ?? '';
        if (value.length > 0) return;
        this.clearAppointmentFilter(false);
    }

    clearAppointmentFilter(refresh = true): void {
        this.selectedAppointment = null;
        this.filterForm.patchValue({ appointmentId: null });
        this.appointmentSearchCtrl.setValue('', { emitEvent: false });
        if (refresh) {
            this.filterData();
        }
    }

    displayAppointment = (appointment: any): string => {
        if (!appointment) return '';
        if (typeof appointment === 'string') return appointment;
        const token = appointment.tokenNumber ? `Token # ${appointment.tokenNumber}` : `Booking # ${appointment.id}`;
        const patientName = appointment?.patient?.patientMaster?.name
            || appointment?.patient?.name
            || '';
        return patientName ? `${token} - ${patientName}` : token;
    };

    private setupAppointmentAutocomplete(): void {
        this.filteredAppointments$ = this.appointmentSearchCtrl.valueChanges.pipe(
            startWith(''),
            debounceTime(300),
            distinctUntilChanged(),
            switchMap((value: string | any) => {
                const term = typeof value === 'string'
                    ? value.trim()
                    : String(value?.tokenNumber ?? value?.id ?? '').trim();
                if (!term) return of([]);
                this.appointmentLoading = true;
                return this.appointmentService.getAppointmentByToken(term, 0).pipe(
                    map((data: any) => data?.item1 ?? data ?? []),
                    finalize(() => (this.appointmentLoading = false))
                );
            })
        );

        this.appointmentSearchCtrl.valueChanges.subscribe((value) => {
            if (typeof value === 'string' && value.trim() === '') {
                this.clearAppointmentFilter(false);
            }
        });
    }

    openTransfusion(element: any = null, isViewMode = false): void {
        this.dialog.open(ProcessTransfusionComponent, {
            panelClass: 'cstm_width_950',
            maxHeight: '95vh',
            height: 'auto',
            data: { element, isViewMode },
            disableClose: true
        }).afterClosed().subscribe(() => this.bindData());
    }

    viewLog(element: any): void {
        this.dialog.open(BloodRequestLogComponent, {
            data: { element },
            panelClass: 'cstm_width_800',
            height: 'auto',
            maxHeight: '90vh',
            disableClose: true
        });
    }

    deleteRequest(element: any): void {
        this.dialog.open(DeleteBloodRequestComponent, {
            panelClass: 'cstm_width_500',
            height: 'auto',
            data: { element },
            disableClose: true
        }).afterClosed().subscribe(() => this.bindData());
    }

    getBloodGroupName(element: any): string {
        return element?.bloodGroupMaster?.name || '';
    }

    getComponentTypeName(element: any): string {
        return element?.bloodComponentType?.name || '';
    }

    getStatusText(status: number): string {
        return this.statusMap[status] || '';
    }

    canDelete(element: any): boolean {
        return element?.status === 1;
    }
}
