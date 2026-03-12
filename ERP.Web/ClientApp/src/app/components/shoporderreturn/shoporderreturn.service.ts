import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { ShopOrderReturnEndPoints } from './shoporderreturn.endpoints';

@Injectable({
    providedIn: 'root'
})

export class ShopOrderReturnService extends BaseService<any> {

    endPointControllerName = "ShopOrderReturn";
    constructor(httpClient: HttpClient, private http: HttpClient, private shopOrderReturnEndPoints: ShopOrderReturnEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllShopOrderReturns(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.shopOrderReturnEndPoints.getAllShopOrderReturns)
            .pipe(map((data: any) => data));
    }

    saveShopOrderReturn(saveShopOrderReturnCommand: any) {
        return this.post(saveShopOrderReturnCommand, this.endPointControllerName + this.shopOrderReturnEndPoints.saveShopOrderReturn)
            .pipe(map((data: any) => data));
    }

    deleteShopOrderReturn(id: number) {
        return this.delete(id, this.endPointControllerName + this.shopOrderReturnEndPoints.deleteShopOrderReturn)
            .pipe();
    }

    getShopOrderReturnCode() {
        return this.get(this.endPointControllerName + this.shopOrderReturnEndPoints.getShopOrderReturnCode)
            .pipe(map((data: any) => data));
    }

    processShopOrderReturn(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.shopOrderReturnEndPoints.processShopOrderReturn)
            .pipe();
    }

    getShopOrderReturnCount(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.shopOrderReturnEndPoints.getShopOrderReturnCount)
            .pipe(map((data: any) => data));
    }

    async getPendingShopOrder(orderId: any, searchParam: any) {
        // Make sure to send the purchaseDemandIds as a JSON array in the body of the request.
        const body = {
            OrderId: orderId,
            searchParam: searchParam
        };

        return this.post(body, this.endPointControllerName + this.shopOrderReturnEndPoints.getPendingShopOrder)
            .pipe();
    }

    async getPendingShopOrderItems(orderId: number, shopOrderReturnId: number) {
        return this.get('?orderId=' + orderId + '&shopOrderReturnId=' + shopOrderReturnId, this.endPointControllerName + this.shopOrderReturnEndPoints.getPendingShopOrderItems)
            .pipe();
    }

}
