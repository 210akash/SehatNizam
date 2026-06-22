import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { ServiceAccountEndPoints } from './serviceaccount.endpoints';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class ServiceAccountService extends BaseService<any> {

    endPointControllerName = "ServiceAccount";
    constructor(httpClient: HttpClient, private http: HttpClient, private serviceAccountEndPoints: ServiceAccountEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    saveServiceAccount(command:any) {
        return this.post(command, this.endPointControllerName + this.serviceAccountEndPoints.saveServiceAccount)
            .pipe(map((data: any) => data));
    }
}
