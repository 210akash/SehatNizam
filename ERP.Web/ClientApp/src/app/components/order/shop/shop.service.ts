import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { ShopEndPoints } from './shop.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class ShopService extends BaseService<any> {

    endPointControllerName = 'Shop';

    constructor(private http: HttpClient, httpClient: HttpClient, private shopEndPoints: ShopEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getShopByName(name: string) {
        return await this.get('?name=' + name, this.endPointControllerName + this.shopEndPoints.getShopByName)
            .pipe(map((data: any) => data));
    }

    async saveShop(createShopForm: any) {
        return await this.post(createShopForm, this.endPointControllerName + this.shopEndPoints.saveShop)
            .pipe(map((data: any) => data));
    }

    async getAllShop(shopFilterForm: any) {
        return await this.post(shopFilterForm, this.endPointControllerName + this.shopEndPoints.getAllShop)
            .pipe(map((data: any) => data));
    }

    async getShopById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.shopEndPoints.getShopById)
            .pipe(map((data: any) => data));
    }

    async deleteShop(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.shopEndPoints.deleteShop)
            .pipe(map((data: any) => data));
    }

    async getShopsByTerritoryId(territoryId: number) {
        return this.get('?territoryId=' + territoryId, this.endPointControllerName + this.shopEndPoints.getShopsByTerritoryId)
            .pipe(map((data: any) => data));
    }

    async getShopsByRouteId(routeId: number) {
        return this.get('?routeId=' + routeId, this.endPointControllerName + this.shopEndPoints.getShopsByRouteId)
            .pipe(map((data: any) => data));
    }

    async verifyShopById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.shopEndPoints.verifyShopById)
            .pipe(map((data: any) => data));
    }

    approveShop(id: number, remarks: string) {
        return this.get(
            `?id=${id}&remarks=${encodeURIComponent(remarks)}`,
            this.endPointControllerName + this.shopEndPoints.approveShop
        ).pipe();
    }

    rejectShop(id: number, remarks: string) {
        return this.get(
            `?id=${id}&remarks=${encodeURIComponent(remarks)}`,
            this.endPointControllerName + this.shopEndPoints.rejectShop
        ).pipe();
    }

    async searchShopsByTerritoryId(territoryId: number, param: string) {
        return this.get('?territoryId=' + territoryId + '&param=' + param, this.endPointControllerName + this.shopEndPoints.searchShopsByTerritoryId)
            .pipe(map((data: any) => data));
    }

}