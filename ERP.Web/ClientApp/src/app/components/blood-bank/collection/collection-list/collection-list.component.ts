import { Component, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
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

    @ViewChild('donorPaginator') donorPaginator!: MatPaginator;
    @ViewChild('collectionPaginator') collectionPaginator!: MatPaginator;
    @ViewChild('donorSort') donorSort!: MatSort;
    @ViewChild('collectionSort') collectionSort!: MatSort;

    constructor(
        private donorService: DonorService,
        private donationService: DonationService,
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
            donorCNIC: ['']
        });
        this.bindDonors();
        this.bindCollections();
    }

    onTabChange(index: number) {
        this.selectedTabIndex = index;
        if (index === 0) {
            this.bindDonors();
        } else {
            this.bindCollections();
        }
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
        const request = {
            ...this.collectionFilterForm.value,
            PagingData: { currentPage: this.collectionCurrentPage, take: this.collectionTake }
        };

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
        });
    }

    editCollection(element: any) {
        this.dialog.open(CollectBloodComponent, {
            panelClass: 'cstm_width_950',
            maxHeight: '95vh',
            height: 'auto',
            data: { element },
            disableClose: true
        }).afterClosed().subscribe(() => this.bindCollections());
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
        }).afterClosed().subscribe(() => this.bindCollections());
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
