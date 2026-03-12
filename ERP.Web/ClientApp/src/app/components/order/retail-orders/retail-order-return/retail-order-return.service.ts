import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { RetailOrderReturnEndPoints } from './retail-order-return.endpoints';
import { BaseService } from '../../../../Service/base.service';
import { environment } from '../../../../../environments/environment';

@Injectable({
    providedIn: 'root'
})

export class RetailOrderReturnService extends BaseService<any> {

    endPointControllerName = "RetailOrderReturn";
    constructor(httpClient: HttpClient, private http: HttpClient, private retailOrderReturnEndPoints: RetailOrderReturnEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllRetailOrderReturns(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.retailOrderReturnEndPoints.getAllRetailOrderReturns)
            .pipe(map((data: any) => data));
    }

    saveRetailOrderReturn(saveRetailOrderReturnCommand: any) {
        return this.post(saveRetailOrderReturnCommand, this.endPointControllerName + this.retailOrderReturnEndPoints.saveRetailOrderReturn)
            .pipe(map((data: any) => data));
    }

    deleteRetailOrderReturn(id: number) {
        return this.delete(id, this.endPointControllerName + this.retailOrderReturnEndPoints.deleteRetailOrderReturn)
            .pipe();
    }

    getRetailOrderReturnCode() {
        return this.get(this.endPointControllerName + this.retailOrderReturnEndPoints.getRetailOrderReturnCode)
            .pipe(map((data: any) => data));
    }

    processRetailOrderReturn(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.retailOrderReturnEndPoints.processRetailOrderReturn)
            .pipe();
    }

    getRetailOrderReturnCount(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.retailOrderReturnEndPoints.getRetailOrderReturnCount)
            .pipe(map((data: any) => data));
    }

    async getPendingRetailOrder(retailOrderId: any, searchParam: any) {
        // Make sure to send the purchaseDemandIds as a JSON array in the body of the request.
        const body = {
            retailOrderId: retailOrderId,
            searchParam: searchParam
        };

        return this.post(body, this.endPointControllerName + this.retailOrderReturnEndPoints.getPendingRetailOrder)
            .pipe();
    }

    async getPendingRetailOrderItems(retailOrderId: number, retailOrderReturnId: number) {
        return this.get('?retailOrderId=' + retailOrderId + '&retailOrderReturnId=' + retailOrderReturnId, this.endPointControllerName + this.retailOrderReturnEndPoints.getPendingRetailOrderItems)
            .pipe();
    }


}