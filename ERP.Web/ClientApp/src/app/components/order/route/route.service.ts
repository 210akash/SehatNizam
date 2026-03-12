import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { RouteEndPoints } from './route.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class RouteService extends BaseService<any> {

    endPointControllerName = 'Route';

    constructor(private http: HttpClient, httpClient: HttpClient, private routeEndPoints: RouteEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getRouteByName(name: string) {
        return await this.get('?name=' + name, this.endPointControllerName + this.routeEndPoints.getRouteByName)
            .pipe(map((data: any) => data));
    }

    async saveRoute(createRouteForm: any) {
        return await this.post(createRouteForm, this.endPointControllerName + this.routeEndPoints.saveRoute)
            .pipe(map((data: any) => data));
    }

    async getAllRoute(routeFilterForm: any) {
        return await this.post(routeFilterForm, this.endPointControllerName + this.routeEndPoints.getAllRoute)
            .pipe(map((data: any) => data));
    }

    async getRouteById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.routeEndPoints.getRouteById)
            .pipe(map((data: any) => data));
    }

    async deleteRoute(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.routeEndPoints.deleteRoute)
            .pipe(map((data: any) => data));
    }

    async addShopsRoute(shopsToAdd: any) {
        return await this.post(shopsToAdd, this.endPointControllerName + this.routeEndPoints.addShopsRoute)
            .pipe(map((data: any) => data));
    }

    async getRouteByDSFTerritory(dsfId: any) {
        return this.get('?dsfId=' + dsfId, this.endPointControllerName + this.routeEndPoints.getRouteByDSFTerritory)
            .pipe(map((data: any) => data));
    }

    async isShopOccupied(shopId: number, routeId: number) {
        return this.get('?shopId=' + shopId + "&routeId=" + routeId, this.endPointControllerName + this.routeEndPoints.isShopOccupied)
            .pipe(map((data: any) => data));
    }

    async deleteRouteShop(routeShopId: number) {
        return this.get('?routeShopId=' + routeShopId, this.endPointControllerName + this.routeEndPoints.deleteRouteShop)
            .pipe(map((data: any) => data));
    }

    async getRoutesByDsfId(dsfId: any, territoryId: any) {
        return this.get('?dsfId=' + dsfId + '&territoryId=' + territoryId, this.endPointControllerName + this.routeEndPoints.getRoutesByDsfId)
            .pipe(map((data: any) => data));
    }

    async addShopsRouteFrequency(shopsRouteFrequency: any) {
        return await this.post(shopsRouteFrequency, this.endPointControllerName + this.routeEndPoints.addShopsRouteFrequency)
            .pipe(map((data: any) => data));
    }

    async getShopRouteFrequencyByTerritoryId(territoryId: any) {
        return await this.get('?territoryId=' + territoryId, this.endPointControllerName + this.routeEndPoints.getShopRouteFrequencyByTerritoryId)
            .pipe(map((data: any) => data));
    }


}