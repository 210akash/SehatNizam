import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { AreaEndPoints } from './area.endpoints';
import { BaseService } from '../../../Service/base.service';
import { environment } from '../../../../environments/environment';

@Injectable({
    providedIn: 'root'
})

export class AreaService extends BaseService<any> {

    endPointControllerName = 'Area';

    constructor(private http: HttpClient, httpClient: HttpClient, private areaEndPoints: AreaEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAreaByName(name: string) {
        return await this.get('?name=' + name, this.endPointControllerName + this.areaEndPoints.getAreaByName)
            .pipe(map((data: any) => data));
    }

    async saveArea(createAreaForm: any) {
        return await this.post(createAreaForm, this.endPointControllerName + this.areaEndPoints.saveArea)
            .pipe(map((data: any) => data));
    }

    async getAllArea(areaFilterForm: any) {
        return await this.post(areaFilterForm, this.endPointControllerName + this.areaEndPoints.getAllArea)
            .pipe(map((data: any) => data));
    }

    async getAreaById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.areaEndPoints.getAreaById)
            .pipe(map((data: any) => data));
    }

    async deleteArea(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.areaEndPoints.deleteArea)
            .pipe(map((data: any) => data));
    }

    async getAreaByZoneId(zoneId: number) {
        return this.get('?zoneId=' + zoneId, this.endPointControllerName + this.areaEndPoints.getAreaByZoneId)
            .pipe(map((data: any) => data));
    }


}