import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { BloodRequestService } from '../blood-request.service';

@Component({
    selector: 'app-blood-request-log',
    templateUrl: './blood-request-log.component.html',
    styleUrl: './blood-request-log.component.css',
    standalone: false
})
export class BloodRequestLogComponent implements OnInit {
    isLoading = true;
    log: any = null;

    constructor(
        private service: BloodRequestService,
        private notificationsService: NotificationsService,
        @Inject(MAT_DIALOG_DATA) public data: { element: any }
    ) { }

    ngOnInit(): void {
        this.service.getLog(this.data.element.id).subscribe({
            next: (log: any) => {
                this.log = log;
                this.isLoading = false;
            },
            error: () => {
                this.notificationsService.showNotification('Failed to load request log', 'snack-bar-danger');
                this.isLoading = false;
            }
        });
    }

    getBloodRequest(): any {
        return this.log?.bloodRequest;
    }

    getBloodGroupName(): string {
        const req = this.getBloodRequest();
        return req?.bloodGroupMaster?.name || req?.bloodGroupMaster?.code || '—';
    }

    getComponentTypeName(): string {
        const req = this.getBloodRequest();
        return req?.bloodComponentType?.name || req?.bloodComponentType?.code || '—';
    }

    getOutcomeClass(outcome: string, isReverted: boolean): string {
        if (isReverted) return 'outcome-reverted';
        if (outcome === 'Compatible' || outcome === 'Issued' || outcome === 'Completed') return 'outcome-success';
        if (outcome === 'Incompatible') return 'outcome-failed';
        if (outcome === 'In Process') return 'outcome-pending';
        return 'outcome-default';
    }

    getCurrentStepClass(): string {
        const step = (this.log?.currentStep || '').toLowerCase();
        if (step.includes('cancelled')) return 'step-cancelled';
        if (step.includes('completed') || step.includes('issued')) return 'step-done';
        if (step.includes('awaiting blood issue') || step.includes('cross matched')) return 'step-ready';
        if (step.includes('in process')) return 'step-progress';
        if (step.includes('pending')) return 'step-pending';
        return 'step-default';
    }

    getCurrentStepIcon(): string {
        const stepClass = this.getCurrentStepClass();
        switch (stepClass) {
            case 'step-done': return 'check_circle';
            case 'step-ready': return 'verified';
            case 'step-progress': return 'hourglass_top';
            case 'step-cancelled': return 'cancel';
            case 'step-pending': return 'pending_actions';
            default: return 'info';
        }
    }

    getStepIcon(step: string): string {
        switch (step) {
            case 'Blood Request': return 'assignment';
            case 'Cross Match': return 'science';
            case 'Cross Match Result': return 'biotech';
            case 'Cross Match Reverted': return 'undo';
            case 'Blood Issue': return 'local_hospital';
            case 'Blood Issue Reverted': return 'history';
            default: return 'event_note';
        }
    }

    trackByEventDate(_index: number, entry: any): string {
        return `${entry.step}-${entry.eventDate}-${entry.outcome}`;
    }
}
