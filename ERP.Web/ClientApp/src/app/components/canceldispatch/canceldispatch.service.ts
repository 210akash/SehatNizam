import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { CancelDispatchEndPoints } from './canceldispatch.endpoints';

@Injectable({
    providedIn: 'root'
})

export class CancelDispatchService extends BaseService<any> {

    endPointControllerName = "CancelDispatch";

    constructor(httpClient: HttpClient, private http: HttpClient, private CancelDispatchEndPoints: CancelDispatchEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllCancelDispatches(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.CancelDispatchEndPoints.getAllCancelDispatches)
            .pipe(map((data: any) => data));
    }

    saveCancelDispatch(saveCancelDispatchCommand: any) {
        return this.post(saveCancelDispatchCommand, this.endPointControllerName + this.CancelDispatchEndPoints.saveCancelDispatch)
            .pipe(map((data: any) => data));
    }

    deleteCancelDispatch(id: number) {
        return this.delete(id, this.endPointControllerName + this.CancelDispatchEndPoints.deleteCancelDispatch)
            .pipe();
    }

    getCancelDispatchById(id: number) {
        return this.get(id, this.endPointControllerName + this.CancelDispatchEndPoints.getCancelDispatchById)
            .pipe(map((data: any) => data));
    }

    getCancelDispatchCode() {
        return this.get(this.endPointControllerName + this.CancelDispatchEndPoints.getCancelDispatchCode)
            .pipe(map((data: any) => data));
    }

    processCancelDispatch(command: any) {
        return this.post(command, this.endPointControllerName + this.CancelDispatchEndPoints.processCancelDispatch)
            .pipe();
    }

    getCancelDispatchCount(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.CancelDispatchEndPoints.getCancelDispatchCount)
            .pipe(map((data: any) => data));
    }

    getPendingCancelOrder(CancelDispatchId: number) {
        return this.get('?CancelDispatchId=' + CancelDispatchId, this.endPointControllerName + this.CancelDispatchEndPoints.getPendingCancelOrder)
            .pipe();
    }

    async getPendingCancelOrderItems(orderId: number, CancelDispatchId: number) {
        return this.get('?orderId=' + orderId + '&CancelDispatchId=' + CancelDispatchId, this.endPointControllerName + this.CancelDispatchEndPoints.getPendingCancelOrderItems)
            .pipe();
    }


}
