import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { SaleReturnEndPoints } from './salereturn.endpoints';

@Injectable({
    providedIn: 'root'
})

export class SaleReturnService extends BaseService<any> {

    endPointControllerName = "SaleReturn";
    constructor(httpClient: HttpClient, private http: HttpClient, private saleReturnEndPoints: SaleReturnEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllSaleReturns(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.saleReturnEndPoints.getAllSaleReturns)
            .pipe(map((data: any) => data));
    }

    saveSaleReturn(saveSaleReturnCommand: any) {
        return this.post(saveSaleReturnCommand, this.endPointControllerName + this.saleReturnEndPoints.saveSaleReturn)
            .pipe(map((data: any) => data));
    }

    deleteSaleReturn(id: number) {
        return this.delete(id, this.endPointControllerName + this.saleReturnEndPoints.deleteSaleReturn)
            .pipe();
    }

    getSaleReturnCode() {
        return this.get(this.endPointControllerName + this.saleReturnEndPoints.getSaleReturnCode)
            .pipe(map((data: any) => data));
    }

    processSaleReturn(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.saleReturnEndPoints.processSaleReturn)
            .pipe();
    }

    getSaleReturnCount(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.saleReturnEndPoints.getSaleReturnCount)
            .pipe(map((data: any) => data));
    }

    approveSaleReturn(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.saleReturnEndPoints.approveSaleReturn)
            .pipe();
    }

    // getPendingDemand(purchaseDemandId: number, saleReturnId: number) {
    //     return this.get('?purchaseDemandId=' + purchaseDemandId + '&saleReturnId=' + saleReturnId, this.endPointControllerName + this.saleReturnEndPoints.getPendingDemand)
    //         .pipe();
    // }

    async getPendingDC(dispatchOrderId: any, searchParam: any) {
        // Make sure to send the purchaseDemandIds as a JSON array in the body of the request.
        const body = {
            DispatchedId: dispatchOrderId,
            searchParam: searchParam
        };

        return this.post(body, this.endPointControllerName + this.saleReturnEndPoints.getPendingDC)
            .pipe();
    }

    async getPendingDCItems(dispatchedId: number, saleReturnId: number) {
        return this.get('?dispatchOrderId=' + dispatchedId + '&saleReturnId=' + saleReturnId, this.endPointControllerName + this.saleReturnEndPoints.getPendingDCItems)
            .pipe();
    }

}
