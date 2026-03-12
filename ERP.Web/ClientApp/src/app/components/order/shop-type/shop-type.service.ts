import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { ShopTypeEndPoints } from './shop-type.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class ShopTypeService extends BaseService<any> {

    endPointControllerName = 'ShopType';

    constructor(private http: HttpClient, httpClient: HttpClient, private shopTypeEndPoints: ShopTypeEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getShopTypeByName(name: string) {
        return await this.get('?name=' + name, this.endPointControllerName + this.shopTypeEndPoints.getShopTypeByName)
            .pipe(map((data: any) => data));
    }

    async saveShopType(createShopTypeForm: any) {
        return await this.post(createShopTypeForm, this.endPointControllerName + this.shopTypeEndPoints.saveShopType)
            .pipe(map((data: any) => data));
    }

    async getAllShopType(shopTypeFilterForm: any) {
        return await this.post(shopTypeFilterForm, this.endPointControllerName + this.shopTypeEndPoints.getAllShopType)
            .pipe(map((data: any) => data));
    }

    async getShopTypeById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.shopTypeEndPoints.getShopTypeById)
            .pipe(map((data: any) => data));
    }

    async deleteShopType(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.shopTypeEndPoints.deleteShopType)
            .pipe(map((data: any) => data));
    }


}