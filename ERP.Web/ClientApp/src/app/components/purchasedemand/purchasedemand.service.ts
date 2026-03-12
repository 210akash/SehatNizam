import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { PurchaseDemandEndPoints } from './purchasedemand.endpoints';

@Injectable({
    providedIn: 'root'
})

export class PurchaseDemandService extends BaseService<any> {

    endPointControllerName = "PurchaseDemand";
    constructor(httpClient: HttpClient, private http: HttpClient, private purchasedemandEndPoints: PurchaseDemandEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllPurchaseDemands(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.purchasedemandEndPoints.getAllPurchaseDemands)
            .pipe(map((data: any) => data));
    }

    savePurchaseDemand(savePurchaseDemandCommand: any) {
        return this.post(savePurchaseDemandCommand, this.endPointControllerName + this.purchasedemandEndPoints.savePurchaseDemand)
            .pipe(map((data: any) => data));
    }

    deletePurchaseDemand(id: number) {
        return this.delete(id, this.endPointControllerName + this.purchasedemandEndPoints.deletePurchaseDemand)
            .pipe();
    }

    getPurchaseDemandById(id: number) {
        return this.get(id, this.endPointControllerName + this.purchasedemandEndPoints.getPurchaseDemandById)
            .pipe(map((data: any) => data));
    }

    getPurchaseDemandByName(name: string) {
        return this.get(name, this.endPointControllerName + this.purchasedemandEndPoints.getPurchaseDemandByName)
            .pipe(map((data: any) => data));
    }

    getPurchaseDemandCode() {
        return this.get(this.endPointControllerName + this.purchasedemandEndPoints.getPurchaseDemandCode)
            .pipe(map((data: any) => data));
    }
    
    processPurchaseDemand(id: number) {
        return this.get('?id='+id, this.endPointControllerName + this.purchasedemandEndPoints.processPurchaseDemand)
            .pipe();
    }

    getIndentRequestCount(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.purchasedemandEndPoints.getIndentRequestCount)
            .pipe(map((data: any) => data));
    }

    approvePurchaseDemand(id: number) {
        return this.get('?id='+id, this.endPointControllerName + this.purchasedemandEndPoints.approvePurchaseDemand)
            .pipe();
    }

    getPendingIndentRequest(indentRequestId:number) {
        return this.get('?indentRequestId=' + indentRequestId,this.endPointControllerName + this.purchasedemandEndPoints.getPendingIndentRequest)
            .pipe();
    }

    async getPendingIndentItems(indentRequestId:number, purchaseDemandId : number) {
        return this.get('?indentRequestId=' + indentRequestId + '&purchaseDemandId=' + purchaseDemandId ,this.endPointControllerName + this.purchasedemandEndPoints.getPendingIndentItems)
            .pipe();
    }
}
