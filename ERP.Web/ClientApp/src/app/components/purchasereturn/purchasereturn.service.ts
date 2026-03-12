import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { PurchaseReturnEndPoints } from './purchasereturn.endpoints';

@Injectable({
    providedIn: 'root'
})

export class PurchaseReturnService extends BaseService<any> {

    endPointControllerName = "PurchaseReturn";
    constructor(httpClient: HttpClient, private http: HttpClient, private purchaseReturnEndPoints: PurchaseReturnEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllPurchaseReturns(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.purchaseReturnEndPoints.getAllPurchaseReturns)
            .pipe(map((data: any) => data));
    }

    savePurchaseReturn(savePurchaseReturnCommand: any) {
        return this.post(savePurchaseReturnCommand, this.endPointControllerName + this.purchaseReturnEndPoints.savePurchaseReturn)
            .pipe(map((data: any) => data));
    }

    deletePurchaseReturn(id: number) {
        return this.delete(id, this.endPointControllerName + this.purchaseReturnEndPoints.deletePurchaseReturn)
            .pipe();
    }

    getPurchaseReturnCode() {
        return this.get(this.endPointControllerName + this.purchaseReturnEndPoints.getPurchaseReturnCode)
            .pipe(map((data: any) => data));
    }

    processPurchaseReturn(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.purchaseReturnEndPoints.processPurchaseReturn)
            .pipe();
    }

    getPurchaseReturnCount(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.purchaseReturnEndPoints.getPurchaseReturnCount)
            .pipe(map((data: any) => data));
    }

    approvePurchaseReturn(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.purchaseReturnEndPoints.approvePurchaseReturn)
            .pipe();
    }

    // getPendingDemand(purchaseDemandId: number, purchaseReturnId: number) {
    //     return this.get('?purchaseDemandId=' + purchaseDemandId + '&purchaseReturnId=' + purchaseReturnId, this.endPointControllerName + this.purchaseReturnEndPoints.getPendingDemand)
    //         .pipe();
    // }

    async getPendingGRN(grnId: any, searchParam: any) {
        // Make sure to send the purchaseDemandIds as a JSON array in the body of the request.
        const body = {
            GRNId: grnId,
            searchParam: searchParam
        };

        return this.post(body, this.endPointControllerName + this.purchaseReturnEndPoints.getPendingGRN)
            .pipe();
    }

    async getPendingGRNItems(grnId: number, purchaseReturnId: number) {
        return this.get('?GRNId=' + grnId + '&purchaseReturnId=' + purchaseReturnId, this.endPointControllerName + this.purchaseReturnEndPoints.getPendingGRNItems)
            .pipe();
    }

}
