import { Component, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { forkJoin } from 'rxjs';
import { BloodGroupService } from '../../blood-group/blood-group.service';
import { ComponentTypeService } from '../../component-type/component-type.service';
import { AddBloodUnitComponent } from '../add-blood-unit/add-blood-unit.component';
import { BloodUnitService } from '../blood-unit.service';
import { DonationService } from '../../donation/donation.service';
import { CollectBloodComponent } from '../../collection/collect-blood/collect-blood.component';

interface StockDashboardStats {
    availableUnits: number;
    nearExpiryUnits: number;
    expiredUnits: number;
    noStorageUnits: number;
    reservedUnits: number;
    missingGroupCount: number;
    missingGroupNames: string;
}

@Component({
    selector: 'app-blood-unit-list',
    templateUrl: './blood-unit-list.component.html',
    styleUrls: ['./blood-unit-list.component.css'],
    standalone: false
})
export class BloodUnitListComponent {
    filterForm!: FormGroup;
    isLoading = false;
    currentPage = 0;
    pageSizeOptions: number[] = [5, 10, 25, 100];
    displayedColumns: string[] = ['unitNo', 'componentType', 'bloodGroup', 'storage', 'status', 'expiry', 'actions'];
    dataSource: any;
    take = 10;
    totalRows = 0;
    componentTypeList: any[] = [];
    bloodGroupList: any[] = [];
    dashboardStats: StockDashboardStats = {
        availableUnits: 0,
        nearExpiryUnits: 0,
        expiredUnits: 0,
        noStorageUnits: 0,
        reservedUnits: 0,
        missingGroupCount: 0,
        missingGroupNames: ''
    };
    statusMap: { [key: number]: string } = {
        1: 'Available',
        2: 'Reserved',
        3: 'Issued',
        4: 'Discarded',
        5: 'Expired'
    };
    screeningStatusMap: { [key: number]: string } = {
        1: 'Pending',
        2: 'Pass',
        3: 'Fail',
        4: 'Deferred'
    };
    statusFilterOptions = [
        { value: 0, name: 'All' },
        { value: 1, name: 'Available' },
        { value: 2, name: 'Reserved' },
        { value: 3, name: 'Issued' },
        { value: 4, name: 'Discarded' },
        { value: 5, name: 'Expired' }
    ];
    storageFilterOptions = [
        { value: 0, name: 'All' },
        { value: 1, name: 'Assigned' },
        { value: 2, name: 'Not Assigned' }
    ];

    @ViewChild(MatPaginator) paginator!: MatPaginator;
    @ViewChild(MatSort) sort!: MatSort;

    constructor(
        private service: BloodUnitService,
        private donationService: DonationService,
        private componentTypeService: ComponentTypeService,
        private bloodGroupService: BloodGroupService,
        private dialog: MatDialog,
        private formBuilder: FormBuilder
    ) { }

    ngOnInit(): void {
        this.filterForm = this.formBuilder.group({
            unitNo: [''],
            componentTypeName: [''],
            bloodGroupMasterId: [null],
            status: [0],
            storageAssigned: [0]
        });
        this.loadLookups();
        this.loadDashboardStats();
        this.bindData();
    }

    get dashboardCards(): Array<{ title: string; value: number | string; subtitle?: string; icon: string; tone: string }> {
        return [
            {
                title: 'Available Stock',
                value: this.dashboardStats.availableUnits,
                icon: 'inventory_2',
                tone: 'primary'
            },
            {
                title: 'Near Expiry (7 days)',
                value: this.dashboardStats.nearExpiryUnits,
                icon: 'schedule',
                tone: 'warning'
            },
            {
                title: 'Expired Units',
                value: this.dashboardStats.expiredUnits,
                icon: 'dangerous',
                tone: 'danger'
            },
            {
                title: 'No Storage',
                value: this.dashboardStats.noStorageUnits,
                icon: 'kitchen',
                tone: 'muted'
            },
            {
                title: 'Missing Groups',
                value: this.dashboardStats.missingGroupCount,
                subtitle: this.dashboardStats.missingGroupNames || 'All groups in stock',
                icon: 'bloodtype',
                tone: 'accent'
            }
        ];
    }

    loadLookups(): void {
        this.componentTypeService.getAll({ PagingData: { currentPage: 0, take: 1000 } }).subscribe((data: any) => {
            this.componentTypeList = data.item1 || [];
        });
        this.bloodGroupService.getAll({ PagingData: { currentPage: 0, take: 1000 } }).subscribe((data: any) => {
            this.bloodGroupList = data.item1 || [];
        });
    }

    private readonly issuedStatus = 3;

    loadDashboardStats(): void {
        forkJoin({
            units: this.service.getAll({
                ExcludeIssued: true,
                PagingData: { currentPage: 0, take: 10000 }
            }),
            groups: this.bloodGroupService.getAll({ PagingData: { currentPage: 0, take: 1000 } })
        }).subscribe({
            next: ({ units, groups }: any) => {
                const inStockUnits = this.filterInStockUnits(units?.item1 || []);
                const available = inStockUnits.filter((unit: any) => unit.status === 1);
                const groupList = groups?.item1 || [];
                const stockedGroupIds = new Set(available.map((unit: any) => unit.bloodGroupMasterId));
                const missingGroups = groupList.filter((group: any) => !stockedGroupIds.has(group.id));

                this.dashboardStats = {
                    availableUnits: available.length,
                    nearExpiryUnits: available.filter((unit: any) => {
                        const days = this.getExpiryDays(unit);
                        return days >= 0 && days <= 7;
                    }).length,
                    expiredUnits: inStockUnits.filter((unit: any) => {
                        if (unit.status === 5) return true;
                        return unit.status === 1 && this.getExpiryDays(unit) < 0;
                    }).length,
                    noStorageUnits: available.filter((unit: any) => !this.isStorageAssigned(unit)).length,
                    reservedUnits: inStockUnits.filter((unit: any) => unit.status === 2).length,
                    missingGroupCount: missingGroups.length,
                    missingGroupNames: missingGroups
                        .map((group: any) => group.name || group.code)
                        .filter(Boolean)
                        .join(', ')
                };
            }
        });
    }

    bindData(): void {
        this.isLoading = true;
        const raw = this.filterForm.value;
        const request: any = {
            UnitNo: raw.unitNo,
            ComponentTypeName: raw.componentTypeName,
            Status: raw.status,
            StorageAssigned: raw.storageAssigned,
            PagingData: { currentPage: this.currentPage, take: this.take }
        };

        if (raw.bloodGroupMasterId) {
            request.BloodGroupMasterId = raw.bloodGroupMasterId;
        }

        this.service.getAll(request).subscribe({
            next: (data: any) => {
                const items = data.item1 || [];
                this.dataSource = new MatTableDataSource(items);
                this.totalRows = data.item2;
                this.dataSource.sort = this.sort;

                if (items.length > 0) {
                    setTimeout(() => {
                        this.paginator.pageIndex = this.currentPage;
                        this.paginator.length = this.totalRows;
                    });
                }

                this.isLoading = false;
            },
            error: () => this.isLoading = false
        });
    }

    private filterInStockUnits(units: any[]): any[] {
        return (units || []).filter((unit) => unit?.status !== this.issuedStatus);
    }

    isIssuedUnit(unit: any): boolean {
        return unit?.status === this.issuedStatus;
    }

    pageChanged(event: PageEvent): void {
        this.take = event.pageSize;
        this.currentPage = event.pageIndex;
        this.bindData();
    }

    filterData() {
        this.currentPage = 0;
        this.bindData();
    }

    viewDialog(element: any): void {
        this.dialog.open(AddBloodUnitComponent, {
            data: { element, isViewMode: true },
            panelClass: 'cstm_width_700',
            height: 'auto',
            disableClose: true
        });
    }

    canUpdateScreeningStatus(element: any): boolean {
        const screeningStatus = element?.bloodDonation?.screeningStatus;
        return element?.status === 1
            && element?.bloodDonationId > 0
            && screeningStatus === 2;
    }

    getDonationScreeningText(element: any): string {
        const status = element?.bloodDonation?.screeningStatus;
        return status ? (this.screeningStatusMap[status] || '') : '';
    }

    updateScreeningStatus(element: any): void {
        const donationId = element?.bloodDonationId || element?.bloodDonation?.id;
        if (!donationId) {
            return;
        }

        this.donationService.getById(donationId).subscribe({
            next: (donation: any) => {
                this.dialog.open(CollectBloodComponent, {
                    panelClass: 'cstm_width_950',
                    maxHeight: '95vh',
                    height: 'auto',
                    data: { element: donation, screeningUpdateOnly: true },
                    disableClose: true
                }).afterClosed().subscribe(() => {
                    this.bindData();
                    this.loadDashboardStats();
                });
            }
        });
    }

    getComponentTypeName(element: any): string {
        return element?.bloodComponentType?.name || '';
    }

    getBloodGroupName(element: any): string {
        return element?.bloodGroupMaster?.name || '';
    }

    getStatusText(status: number): string {
        return this.statusMap[status] || '';
    }

    isStorageAssigned(element: any): boolean {
        return !!(element?.bloodFridgeId && element?.bloodRackId);
    }

    getStorageText(element: any): string {
        if (!this.isStorageAssigned(element)) {
            return '';
        }

        const fridge = element?.bloodFridge?.name || element?.bloodFridge?.code || 'Fridge';
        const rack = element?.bloodRack?.name || element?.bloodRack?.code || 'Rack';
        const slot = element?.slotNo ? `Slot ${element.slotNo}` : '';
        return [fridge, rack, slot].filter(Boolean).join(' | ');
    }

    getExpiryDays(unit: any): number {
        if (this.isIssuedUnit(unit)) return 0;
        if (!unit?.expiryDate) return 0;
        const expiry = new Date(unit.expiryDate);
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        expiry.setHours(0, 0, 0, 0);
        return Math.ceil((expiry.getTime() - today.getTime()) / (1000 * 60 * 60 * 24));
    }

    getExpiryDaysText(unit: any): string {
        if (this.isIssuedUnit(unit)) return '';
        const days = this.getExpiryDays(unit);
        if (days < 0) return `Expired ${Math.abs(days)}d ago`;
        if (days === 0) return 'Expires today';
        return `${days} days left`;
    }
}
