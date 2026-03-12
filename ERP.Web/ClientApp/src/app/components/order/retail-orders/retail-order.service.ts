import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';
import { RetailOrderEndPoints } from './retail-order.endpoints';

@Injectable({
    providedIn: 'root'
})

export class RetailOrderService extends BaseService<any> {

    endPointControllerName = 'RetailOrder';

    constructor(private http: HttpClient, httpClient: HttpClient, private retailOrderEndPoints: RetailOrderEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async saveRetailOrder(createRetailOrderForm: any) {
        return await this.post(createRetailOrderForm, this.endPointControllerName + this.retailOrderEndPoints.saveRetailOrder)
            .pipe(map((data: any) => data));
    }

    async getAllRetailOrder(shopOrderFilterForm: any) {
        return await this.post(shopOrderFilterForm, this.endPointControllerName + this.retailOrderEndPoints.getAllRetailOrder)
            .pipe(map((data: any) => data));
    }

    async getRetailOrderById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.retailOrderEndPoints.getRetailOrderById)
            .pipe(map((data: any) => data));
    }

    async deleteRetailOrder(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.retailOrderEndPoints.deleteRetailOrder)
            .pipe(map((data: any) => data));
    }

    async updateRetailOrderStatus(shopOrderStatusForm: any) {
        return this.post(shopOrderStatusForm, this.endPointControllerName + this.retailOrderEndPoints.updateRetailOrderStatus)
            .pipe(map((data: any) => data));
    }

    async ConfirmRetailOrderQuantity(shopOrderStatusForm: any) {
        return this.post(shopOrderStatusForm, this.endPointControllerName + this.retailOrderEndPoints.confirmRetailOrderQuantity)
            .pipe(map((data: any) => data));
    }

    async getKCItemsByDistributor() {
        return this.get(this.endPointControllerName + this.retailOrderEndPoints.getKCItemsByDistributor)
            .pipe(map((data: any) => data));
    }


}