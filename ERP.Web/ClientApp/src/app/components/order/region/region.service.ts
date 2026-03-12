import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { RegionEndPoints } from './region.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class RegionService extends BaseService<any> {

    endPointControllerName = 'Region';

    constructor(private http: HttpClient, httpClient: HttpClient, private regionEndPoints: RegionEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getRegionByName(name: string) {
        return await this.get('?name=' + name, this.endPointControllerName + this.regionEndPoints.getRegionByName)
            .pipe(map((data: any) => data));
    }

    async saveRegion(createRegionForm: any) {
        return await this.post(createRegionForm, this.endPointControllerName + this.regionEndPoints.saveRegion)
            .pipe(map((data: any) => data));
    }

    async getAllRegion(regionFilterForm: any) {
        return await this.post(regionFilterForm, this.endPointControllerName + this.regionEndPoints.getAllRegion)
            .pipe(map((data: any) => data));
    }

    async getRegionById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.regionEndPoints.getRegionById)
            .pipe(map((data: any) => data));
    }

    async deleteRegion(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.regionEndPoints.deleteRegion)
            .pipe(map((data: any) => data));
    }


}