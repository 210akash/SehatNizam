import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { DSFEndPoints } from './DSF.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class DSFService extends BaseService<any> {

    endPointControllerName = 'DSF';

    constructor(private http: HttpClient, httpClient: HttpClient, private dSFEndPoints: DSFEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAll(dSFFilterForm: any) {
        return await this.post(dSFFilterForm, this.endPointControllerName + this.dSFEndPoints.getAll)
            .pipe(map((data: any) => data));
    }

    async addDSFRoute(dSFRouteForm: any) {
        return await this.post(dSFRouteForm, this.endPointControllerName + this.dSFEndPoints.addDSFRoute)
            .pipe(map((data: any) => data));
    }


}