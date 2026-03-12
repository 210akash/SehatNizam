import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { PrimaryOrderEndPoints } from './order.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class PrimaryOrderService extends BaseService<any> {

    endPointControllerName = 'PrimaryOrder';

    constructor(private http: HttpClient, httpClient: HttpClient, private orderEndPoints: PrimaryOrderEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async saveOrder(createOrderForm: any) {
        return await this.post(createOrderForm, this.endPointControllerName + this.orderEndPoints.saveOrder)
            .pipe(map((data: any) => data));
    }

    async getAllOrder(orderFilterForm: any) {
        return await this.post(orderFilterForm, this.endPointControllerName + this.orderEndPoints.getAllOrder)
            .pipe(map((data: any) => data));
    }

    async getOrderById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.orderEndPoints.getOrderById)
            .pipe(map((data: any) => data));
    }

    async deleteOrder(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.orderEndPoints.deleteOrder)
            .pipe(map((data: any) => data));
    }

    async updateOrderStatus(orderStatusForm: any) {
        return this.post(orderStatusForm, this.endPointControllerName + this.orderEndPoints.updateOrderStatus)
            .pipe(map((data: any) => data));
    }

    async getAllOrderStatus() {
        return this.get(this.endPointControllerName + this.orderEndPoints.getAllOrderStatus)
            .pipe(map((data: any) => data));
    }


}