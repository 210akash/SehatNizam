import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { IGPEndPoints } from './igp.endpoints';

@Injectable({
    providedIn: 'root'
})

export class IGPService extends BaseService<any> {

    endPointControllerName = "IGP";
    constructor(httpClient: HttpClient, private http: HttpClient, private iGPEndPoints: IGPEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllIGPs(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.iGPEndPoints.getAllIGPs)
            .pipe(map((data: any) => data));
    }

    saveIGP(saveIGPCommand: any) {
        return this.post(saveIGPCommand, this.endPointControllerName + this.iGPEndPoints.saveIGP)
            .pipe(map((data: any) => data));
    }

    deleteIGP(id: number) {
        return this.delete(id, this.endPointControllerName + this.iGPEndPoints.deleteIGP)
            .pipe();
    }

    getIGPById(id: number) {
        return this.get(id, this.endPointControllerName + this.iGPEndPoints.getIGPById)
            .pipe(map((data: any) => data));
    }

    getIGPByName(name: string) {
        return this.get(name, this.endPointControllerName + this.iGPEndPoints.getIGPByName)
            .pipe(map((data: any) => data));
    }

    getIGPCode() {
        return this.get(this.endPointControllerName + this.iGPEndPoints.getIGPCode)
            .pipe(map((data: any) => data));
    }

    processIGP(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.iGPEndPoints.processIGP)
            .pipe();
    }

    getIndentRequestCount(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.iGPEndPoints.getIndentRequestCount)
            .pipe(map((data: any) => data));
    }

    // approveIGP(id: number) {
    //     return this.get('?id=' + id, this.endPointControllerName + this.iGPEndPoints.approveIGP)
    //         .pipe();
    // }

    // getPendingDemand(purchaseDemandId: number, iGPId: number) {
    //     return this.get('?purchaseDemandId=' + purchaseDemandId + '&iGPId=' + iGPId, this.endPointControllerName + this.iGPEndPoints.getPendingDemand)
    //         .pipe();
    // }

    getPendingPurchaseOrders(purchaseOrderId: any) {
        return this.get('?purchaseOrderId=' + purchaseOrderId, this.endPointControllerName + this.iGPEndPoints.getPendingPurchaseOrders)
            .pipe();
    }

    async getPendingPOItems(purchaseOrderId: number) {
        return this.get('?purchaseOrderId=' + purchaseOrderId, this.endPointControllerName + this.iGPEndPoints.getPendingPOItems)
            .pipe();
    }

}
