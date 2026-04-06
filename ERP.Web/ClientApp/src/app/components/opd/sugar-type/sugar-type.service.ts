import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { SugarTypeEndPoints } from './sugar-type.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class SugarTypeService extends BaseService<any> {

    endPointControllerName = 'SugarType';

    constructor(private http: HttpClient, httpClient: HttpClient, private sugarTypeEndPoints: SugarTypeEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async saveSugarType(createSugarTypeForm: any) {
        return await this.post(createSugarTypeForm, this.endPointControllerName + this.sugarTypeEndPoints.saveSugarType)
            .pipe(map((data: any) => data));
    }

    async getAllSugarType(sugarTypeFilterForm: any) {
        return await this.post(sugarTypeFilterForm, this.endPointControllerName + this.sugarTypeEndPoints.getAllSugarType)
            .pipe(map((data: any) => data));
    }

    async deleteSugarType(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.sugarTypeEndPoints.deleteSugarType)
            .pipe(map((data: any) => data));
    }
}