import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { PurchaseOrderEndPoints } from './purchaseorder.endpoints';

@Injectable({
    providedIn: 'root'
})

export class PurchaseOrderService extends BaseService<any> {

    endPointControllerName = "PurchaseOrder";
    constructor(httpClient: HttpClient, private http: HttpClient, private purchaseorderEndPoints: PurchaseOrderEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllPurchaseOrders(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.purchaseorderEndPoints.getAllPurchaseOrders)
            .pipe(map((data: any) => data));
    }

    savePurchaseOrder(savePurchaseOrderCommand: any) {
        return this.post(savePurchaseOrderCommand, this.endPointControllerName + this.purchaseorderEndPoints.savePurchaseOrder)
            .pipe(map((data: any) => data));
    }

    deletePurchaseOrder(id: number) {
        return this.delete(id, this.endPointControllerName + this.purchaseorderEndPoints.deletePurchaseOrder)
            .pipe();
    }

    getPurchaseOrderById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.purchaseorderEndPoints.getPurchaseOrderById)
            .pipe(map((data: any) => data));
    }

    getPurchaseOrderByName(name: string) {
        return this.get(name, this.endPointControllerName + this.purchaseorderEndPoints.getPurchaseOrderByName)
            .pipe(map((data: any) => data));
    }

    getPurchaseOrderCode() {
        return this.get(this.endPointControllerName + this.purchaseorderEndPoints.getPurchaseOrderCode)
            .pipe(map((data: any) => data));
    }

    processPurchaseOrder(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.purchaseorderEndPoints.processPurchaseOrder)
            .pipe();
    }

    getIndentRequestCount(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.purchaseorderEndPoints.getPurchaseOrderCount)
            .pipe(map((data: any) => data));
    }

    approvePurchaseOrder(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.purchaseorderEndPoints.approvePurchaseOrder)
            .pipe();
    }

    async getPendingDemand(purchaseDemandId: number) {
        return this.get('?purchaseDemandId=' + purchaseDemandId, this.endPointControllerName + this.purchaseorderEndPoints.getPendingDemand)
            .pipe();
    }

    async getPendingIndentItems(purchaseDemandId: number, purchaseOrderId: number, vendorId: number) {
        return this.get('?purchaseDemandId=' + purchaseDemandId + '&purchaseOrderId=' + purchaseOrderId + '&vendorId=' + vendorId, this.endPointControllerName + this.purchaseorderEndPoints.getPendingDemandItems)
            .pipe();
    }
}
