import { Component, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, finalize, map, startWith, switchMap } from 'rxjs/operators';
import { AppointmentService } from '../../../opd/appointment/appointment.service';
import { AddDonorComponent } from '../../donor/add-donor/add-donor.component';
import { DeleteDonorComponent } from '../../donor/delete-donor/delete-donor.component';
import { DonorService } from '../../donor/donor.service';
import { DeleteDonationComponent } from '../../donation/delete-donation/delete-donation.component';
import { DonationService } from '../../donation/donation.service';
import { CollectBloodComponent } from '../collect-blood/collect-blood.component';

@Component({
    selector: 'app-collection-list',
    templateUrl: './collection-list.component.html',
    styleUrls: ['./collection-list.component.css'],
    standalone: false
})
export class CollectionListComponent {
    selectedTabIndex = 0;
    donorFilterForm!: FormGroup;
    collectionFilterForm!: FormGroup;
    appointmentSearchCtrl = new FormControl<string | any>('');
    filteredAppointments$!: Observable<any[]>;
    selectedAppointment: any = null;
    appointmentLoading = false;
    isDonorLoading = false;
    isCollectionLoading = false;
    donorCurrentPage = 0;
    collectionCurrentPage = 0;
    pageSizeOptions: number[] = [5, 10, 25, 100];
    donorDisplayedColumns: string[] = ['code', 'name', 'cnic', 'mobile', 'bloodGroup', 'isDeferred', 'actions'];
    collectionDisplayedColumns: string[] = ['code', 'patient', 'donor', 'componentType', 'donationDate', 'screeningStatus', 'actions'];
    donorDataSource: any;
    collectionDataSource: any;
    donorTake = 10;
    collectionTake = 10;
    donorTotalRows = 0;
    collectionTotalRows = 0;
    screeningStatusMap: { [key: number]: string } = {
        1: 'Pending',
        2: 'Pass',
        3: 'Fail',
        4: 'Deferred'
    };
    screeningStatusOptions = [
        { value: 1, name: 'Pending' },
        { value: 2, name: 'Pass' },
        { value: 3, name: 'Fail' },
        { value: 4, name: 'Deferred' }
    ];
    usedDonorIds = new Set<number>();

    @ViewChild('donorPaginator') donorPaginator!: MatPaginator;
    @ViewChild('collectionPaginator') collectionPaginator!: MatPaginator;
    @ViewChild('donorSort') donorSort!: MatSort;
    @ViewChild('collectionSort') collectionSort!: MatSort;

    constructor(
        private donorService: DonorService,
        private donationService: DonationService,
        private appointmentService: AppointmentService,
        private dialog: MatDialog,
        private formBuilder: FormBuilder
    ) { }

    ngOnInit(): void {
        this.donorFilterForm = this.formBuilder.group({
            name: [''],
            cnic: ['']
        });
        this.collectionFilterForm = this.formBuilder.group({
            donorName: [''],
            donorCNIC: [''],
            screeningStatus: [null],
            appointmentId: [null]
        });
        this.setupAppointmentAutocomplete();
        this.bindDonors();
        this.bindCollections();
        this.loadUsedDonorIds();
    }

    onTabChange(index: number) {
        this.selectedTabIndex = index;
        if (index === 0) {
            this.bindCollections();
        } else {
            this.bindDonors();
        }
    }

    private loadUsedDonorIds(): void {
        this.donationService.getAll({
            PagingData: { currentPage: 0, take: 10000 }
        }).subscribe({
            next: (data: any) => {
                this.usedDonorIds = new Set(
                    (data.item1 || [])
                        .map((item: any) => item.bloodDonorId)
                        .filter((id: number) => id > 0)
                );
            }
        });
    }

    isDonorUsedInCollection(donor: any): boolean {
        return this.usedDonorIds.has(donor?.id);
    }

    canDeleteCollection(element: any): boolean {
        return element?.screeningStatus === 1;
    }

    bindDonors(): void {
        this.isDonorLoading = true;
        const request = {
            ...this.donorFilterForm.value,
            PagingData: { currentPage: this.donorCurrentPage, take: this.donorTake }
        };

        this.donorService.getAll(request).subscribe({
            next: (data: any) => {
                this.donorDataSource = new MatTableDataSource(data.item1);
                this.donorTotalRows = data.item2;
                this.donorDataSource.sort = this.donorSort;

                if (data.item1.length > 0) {
                    setTimeout(() => {
                        this.donorPaginator.pageIndex = this.donorCurrentPage;
                        this.donorPaginator.length = this.donorTotalRows;
                    });
                }

                this.isDonorLoading = false;
            },
            error: () => this.isDonorLoading = false
        });
    }

    bindCollections(): void {
        this.isCollectionLoading = true;
        const raw = this.collectionFilterForm.value;
        const request: any = {
            donorName: raw.donorName,
            donorCNIC: raw.donorCNIC,
            PagingData: { currentPage: this.collectionCurrentPage, take: this.collectionTake }
        };

        if (raw.screeningStatus) {
            request.screeningStatus = raw.screeningStatus;
        }
        if (raw.appointmentId) {
            request.appointmentId = raw.appointmentId;
        }

        this.donationService.getAll(request).subscribe({
            next: (data: any) => {
                this.collectionDataSource = new MatTableDataSource(data.item1);
                this.collectionTotalRows = data.item2;
                this.collectionDataSource.sort = this.collectionSort;

                if (data.item1.length > 0) {
                    setTimeout(() => {
                        this.collectionPaginator.pageIndex = this.collectionCurrentPage;
                        this.collectionPaginator.length = this.collectionTotalRows;
                    });
                }

                this.isCollectionLoading = false;
            },
            error: () => this.isCollectionLoading = false
        });
    }

    donorPageChanged(event: PageEvent): void {
        this.donorTake = event.pageSize;
        this.donorCurrentPage = event.pageIndex;
        this.bindDonors();
    }

    collectionPageChanged(event: PageEvent): void {
        this.collectionTake = event.pageSize;
        this.collectionCurrentPage = event.pageIndex;
        this.bindCollections();
    }

    filterDonors() {
        this.donorCurrentPage = 0;
        this.bindDonors();
    }

    filterCollections() {
        this.collectionCurrentPage = 0;
        this.bindCollections();
    }

    onAppointmentSelected(appointment: any): void {
        if (!appointment?.id) return;
        this.selectedAppointment = appointment;
        this.appointmentSearchCtrl.setValue(appointment, { emitEvent: false });
        this.collectionFilterForm.patchValue({ appointmentId: appointment.id });
        this.filterCollections();
    }

    onAppointmentInputCleared(event: Event): void {
        const value = (event.target as HTMLInputElement)?.value?.trim() ?? '';
        if (value.length > 0) return;
        this.clearAppointmentFilter(false);
    }

    clearAppointmentFilter(refresh = true): void {
        this.selectedAppointment = null;
        this.collectionFilterForm.patchValue({ appointmentId: null });
        this.appointmentSearchCtrl.setValue('', { emitEvent: false });
        if (refresh) {
            this.filterCollections();
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

    openAddDonor(element: any = null) {
        this.dialog.open(AddDonorComponent, {
            panelClass: 'cstm_width_700',
            height: 'auto',
            data: { element },
            disableClose: true
        }).afterClosed().subscribe(() => this.bindDonors());
    }

    viewDonor(element: any) {
        this.dialog.open(AddDonorComponent, {
            data: { element, isViewMode: true },
            panelClass: 'cstm_width_700',
            height: 'auto',
            disableClose: true
        });
    }

    deleteDonor(element: any) {
        this.dialog.open(DeleteDonorComponent, {
            panelClass: 'cstm_width_500',
            height: 'auto',
            data: { element },
            disableClose: true
        }).afterClosed().subscribe(() => this.bindDonors());
    }

    openCollectBlood(donor: any = null) {
        this.dialog.open(CollectBloodComponent, {
            panelClass: 'cstm_width_950',
            maxHeight: '95vh',
            height: 'auto',
            data: { donor },
            disableClose: true
        }).afterClosed().subscribe(() => {
            this.bindDonors();
            this.bindCollections();
            this.loadUsedDonorIds();
        });
    }

    editCollection(element: any) {
        this.dialog.open(CollectBloodComponent, {
            panelClass: 'cstm_width_950',
            maxHeight: '95vh',
            height: 'auto',
            data: { element },
            disableClose: true
        }).afterClosed().subscribe(() => {
            this.bindCollections();
            this.loadUsedDonorIds();
        });
    }

    viewCollection(element: any) {
        this.dialog.open(CollectBloodComponent, {
            panelClass: 'cstm_width_950',
            maxHeight: '95vh',
            height: 'auto',
            data: { element, isViewMode: true },
            disableClose: true
        });
    }

    deleteCollection(element: any) {
        this.dialog.open(DeleteDonationComponent, {
            panelClass: 'cstm_width_500',
            height: 'auto',
            data: { element },
            disableClose: true
        }).afterClosed().subscribe(() => {
            this.bindCollections();
            this.loadUsedDonorIds();
        });
    }

    getBloodGroupName(element: any): string {
        return element?.bloodGroupMaster?.name || '';
    }

    getDonorName(element: any): string {
        return element?.bloodDonor?.name || '';
    }

    getPatientName(element: any): string {
        return element?.appointment?.patient?.patientMaster?.name
            || element?.appointment?.patient?.name
            || 'Independent';
    }

    getAppointmentToken(element: any): string {
        if (!element?.appointment) return '';
        return element.appointment.tokenNumber
            ? `Token # ${element.appointment.tokenNumber}`
            : `Booking # ${element.appointment.id}`;
    }

    getComponentTypeName(element: any): string {
        return element?.bloodComponentType?.name || '';
    }

    getScreeningStatusText(status: number): string {
        return this.screeningStatusMap[status] || '';
    }
}
