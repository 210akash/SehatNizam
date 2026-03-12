import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { IssuanceEndPoints } from './issuance.endpoints';

@Injectable({
    providedIn: 'root'
})

export class IssuanceService extends BaseService<any> {

    endPointControllerName = "Issuance";
    constructor(httpClient: HttpClient, private http: HttpClient, private issuanceEndPoints: IssuanceEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllIssuances(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.issuanceEndPoints.getAllIssuances)
            .pipe(map((data: any) => data));
    }

    saveIssuance(saveIssuanceCommand: any) {
        return this.post(saveIssuanceCommand, this.endPointControllerName + this.issuanceEndPoints.saveIssuance)
            .pipe(map((data: any) => data));
    }

    deleteIssuance(id: number) {
        return this.delete(id, this.endPointControllerName + this.issuanceEndPoints.deleteIssuance)
            .pipe();
    }

    getIssuanceById(id: number) {
        return this.get(id, this.endPointControllerName + this.issuanceEndPoints.getIssuanceById)
            .pipe(map((data: any) => data));
    }

    getIssuanceByName(name: string) {
        return this.get(name, this.endPointControllerName + this.issuanceEndPoints.getIssuanceByName)
            .pipe(map((data: any) => data));
    }

    getIssuanceCode() {
        return this.get(this.endPointControllerName + this.issuanceEndPoints.getIssuanceCode)
            .pipe(map((data: any) => data));
    }

    processIssuance(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.issuanceEndPoints.processIssuance)
            .pipe();
    }

    getIndentRequestCount(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.issuanceEndPoints.getIndentRequestCount)
            .pipe(map((data: any) => data));
    }

    approveIssuance(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.issuanceEndPoints.approveIssuance)
            .pipe();
    }

    // getPendingDemand(purchaseDemandId: number, issuanceId: number) {
    //     return this.get('?purchaseDemandId=' + purchaseDemandId + '&issuanceId=' + issuanceId, this.endPointControllerName + this.issuanceEndPoints.getPendingDemand)
    //         .pipe();
    // }

    getPendingIndentRequest(indentRequestId: any) {
        return this.get('?IndentRequestId=' + indentRequestId, this.endPointControllerName + this.issuanceEndPoints.getPendingIndentRequest)
            .pipe();
    }

    async getPendingIndentRequestItems(indentRequestId: number, issuanceId: number) {
        return this.get('?indentRequestId=' + indentRequestId + '&issuanceId=' + issuanceId, this.endPointControllerName + this.issuanceEndPoints.getPendingIndentRequestItems)
        .pipe();
    }
}
