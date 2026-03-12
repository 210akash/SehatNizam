import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { AuditReviewEndPoints } from './auditreview.endpoints';

@Injectable({
    providedIn: 'root'
})

export class AuditReviewService extends BaseService<any> {

    endPointControllerName = "AuditReview";
    constructor(httpClient: HttpClient, private http: HttpClient, private auditreviewEndPoints: AuditReviewEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllAuditReviews(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.auditreviewEndPoints.getAllAuditReviews)
            .pipe(map((data: any) => data));
    }

    saveAuditReview(saveAuditReviewCommand: any) {
        return this.post(saveAuditReviewCommand, this.endPointControllerName + this.auditreviewEndPoints.saveAuditReview)
            .pipe(map((data: any) => data));
    }

    deleteAuditReview(id: number) {
        return this.delete(id, this.endPointControllerName + this.auditreviewEndPoints.deleteAuditReview)
            .pipe();
    }

    getAuditReviewById(id: number) {
        return this.get(id, this.endPointControllerName + this.auditreviewEndPoints.getAuditReviewById)
            .pipe(map((data: any) => data));
    }

    getAuditReviewByName(name: string) {
        return this.get(name, this.endPointControllerName + this.auditreviewEndPoints.getAuditReviewByName)
            .pipe(map((data: any) => data));
    }

    getAuditReviewCode() {
        return this.get(this.endPointControllerName + this.auditreviewEndPoints.getAuditReviewCode)
            .pipe(map((data: any) => data));
    }

    processAuditReview(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.auditreviewEndPoints.processAuditReview)
            .pipe();
    }

    getAuditReviewCount(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.auditreviewEndPoints.getAuditReviewCount)
            .pipe(map((data: any) => data));
    }

    approveAuditReview(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.auditreviewEndPoints.approveAuditReview)
            .pipe();
    }

    // getPendingDemand(purchaseDemandId: number, auditreviewId: number) {
    //     return this.get('?purchaseDemandId=' + purchaseDemandId + '&auditreviewId=' + auditreviewId, this.endPointControllerName + this.auditreviewEndPoints.getPendingDemand)
    //         .pipe();
    // }

    getPendingInspection(inspectionId: any) {
        return this.get('?inspectionId=' + inspectionId, this.endPointControllerName + this.auditreviewEndPoints.getPendingInspection)
            .pipe();
    }

    async getPendingInspectionItems(inspectionId: number,auditreviewId :number) {
        return this.get('?inspectionId=' + inspectionId + '&auditreviewId=' + auditreviewId , this.endPointControllerName + this.auditreviewEndPoints.getPendingInspectionItems)
            .pipe();
    }

    saveAuditReviewTransaction(saveAuditReviewTransactionCommand: any) {
        return this.post(saveAuditReviewTransactionCommand, this.endPointControllerName + this.auditreviewEndPoints.saveAuditReviewTransaction)
            .pipe(map((data: any) => data));
    }

        revokeAuditReview(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.auditreviewEndPoints.revokeAuditReview)
            .pipe();
    }
}
