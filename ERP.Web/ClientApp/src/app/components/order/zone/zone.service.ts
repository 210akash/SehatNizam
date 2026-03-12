import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { ZoneEndPoints } from './zone.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class ZoneService extends BaseService<any> {

    endPointControllerName = 'Zone';

    constructor(private http: HttpClient, httpClient: HttpClient, private zoneEndPoints: ZoneEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getZoneByName(name: string) {
        return await this.get('?name=' + name, this.endPointControllerName + this.zoneEndPoints.getZoneByName)
            .pipe(map((data: any) => data));
    }

    async saveZone(createZoneForm: any) {
        return await this.post(createZoneForm, this.endPointControllerName + this.zoneEndPoints.saveZone)
            .pipe(map((data: any) => data));
    }

    async getAllZone(zoneFilterForm: any) {
        return await this.post(zoneFilterForm, this.endPointControllerName + this.zoneEndPoints.getAllZone)
            .pipe(map((data: any) => data));
    }

    async getZoneById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.zoneEndPoints.getZoneById)
            .pipe(map((data: any) => data));
    }

    async deleteZone(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.zoneEndPoints.deleteZone)
            .pipe(map((data: any) => data));
    }

    async getFieldMapFilter(fileMapFilterForm: any) {
        return await this.post(fileMapFilterForm, this.endPointControllerName + this.zoneEndPoints.getFieldMapFilter)
            .pipe(map((data: any) => data));
    }

    async getZoneByRegionId(regionId: number) {
        return this.get('?regionId=' + regionId, this.endPointControllerName + this.zoneEndPoints.getZoneByRegionId)
            .pipe(map((data: any) => data));
    }


}