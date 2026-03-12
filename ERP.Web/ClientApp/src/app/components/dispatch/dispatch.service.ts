import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { DispatchEndPoints } from './dispatch.endpoints';

@Injectable({
    providedIn: 'root'
})

export class DispatchService extends BaseService<any> {

    endPointControllerName = "Dispatch";
    endPointControllerNameTemplate = "Templates";

    constructor(httpClient: HttpClient, private http: HttpClient, private dispatchEndPoints: DispatchEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllDispatchs(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.dispatchEndPoints.getAllDispatchs)
            .pipe(map((data: any) => data));
    }

    saveDispatch(saveDispatchCommand: any) {
        return this.post(saveDispatchCommand, this.endPointControllerName + this.dispatchEndPoints.saveDispatch)
            .pipe(map((data: any) => data));
    }

    deleteDispatch(id: number) {
        return this.delete(id, this.endPointControllerName + this.dispatchEndPoints.deleteDispatch)
            .pipe();
    }

    getDispatchById(id: number) {
        return this.get(id, this.endPointControllerName + this.dispatchEndPoints.getDispatchById)
            .pipe(map((data: any) => data));
    }

    getDispatchByName(name: string) {
        return this.get(name, this.endPointControllerName + this.dispatchEndPoints.getDispatchByName)
            .pipe(map((data: any) => data));
    }

    getDispatchCode() {
        return this.get(this.endPointControllerName + this.dispatchEndPoints.getDispatchCode)
            .pipe(map((data: any) => data));
    }

    processDispatch(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.dispatchEndPoints.processDispatch)
            .pipe();
    }

    getIndentRequestCount(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.dispatchEndPoints.getIndentRequestCount)
            .pipe(map((data: any) => data));
    }

    approveDispatch(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.dispatchEndPoints.approveDispatch)
            .pipe();
    }

    async getPendingOrder(OrderIds: number[], searchParam: any) {
        // Make sure to send the purchaseDemandIds as a JSON array in the body of the request.
        const body = {
            OrderId: OrderIds,
            searchParam: searchParam
        };

        return this.post(body, this.endPointControllerName + this.dispatchEndPoints.getPendingOrder)
            .pipe();
    }

    async getPendingOrderItems(orderId: number, dispatchId: number) {
        return this.get('?orderId=' + orderId + '&dispatchId=' + dispatchId, this.endPointControllerName + this.dispatchEndPoints.getPendingOrderItems)
            .pipe();
    }

    getOrderPrint(orderId: number, templateId: number, dispatchId: number) {
        return this.get('?orderId=' + orderId + '&templateId=' + templateId + '&dispatchId=' + dispatchId, this.endPointControllerNameTemplate + this.dispatchEndPoints.getPrintTemplate)
            .pipe();
    }

    getDispatchByOrderId(orderId: number) {
        return this.get('?orderId=' + orderId, this.endPointControllerName + this.dispatchEndPoints.getDispatchByOrderId)
            .pipe(map((data: any) => data));
    }

    receiveDispatchOrder(dispatchOrderId: number) {
        return this.get('?dispatchOrderId=' + dispatchOrderId, this.endPointControllerName + this.dispatchEndPoints.receiveDispatchOrder)
            .pipe(map((data: any) => data));
    }

    updateDispatchPrintStatus(dispatchForm: any) {
        return this.post(dispatchForm, this.endPointControllerName + this.dispatchEndPoints.updateDispatchPrintStatus)
            .pipe(map((data: any) => data));
    }

    async getOrdersToDispatch(_orderListFilerForm: any) {
        return this.post(_orderListFilerForm, this.endPointControllerName + this.dispatchEndPoints.getOrdersToDispatch)
            .pipe(map((data: any) => data));
    }

    getPendingCostSheet(itemId: number, projectId: number, costSheetId: number) {
        return this.get('?itemId=' + itemId + '&projectId=' + projectId + '&costSheetId=' + costSheetId, this.endPointControllerName + this.dispatchEndPoints.getPendingCostSheet)
            .pipe();
    }

    rejectDispatch(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.dispatchEndPoints.rejectDispatch)
            .pipe();
    }

    
}