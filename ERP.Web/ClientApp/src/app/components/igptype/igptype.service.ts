import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { IGPTypeEndPoints } from './igpigptype.endpoints';

@Injectable({
    providedIn: 'root'
})

export class IGPTypeService extends BaseService<any> {

    endPointControllerName = "IGPType";
    constructor(httpClient: HttpClient, private http: HttpClient, private iGPTypeEndPoints: IGPTypeEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllIGPType(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.iGPTypeEndPoints.getAllIGPType)
            .pipe(map((data: any) => data));
    }

    saveIGP(saveIGPTypeCommand: any) {
        return this.post(saveIGPTypeCommand, this.endPointControllerName + this.iGPTypeEndPoints.saveIGPType)
            .pipe(map((data: any) => data));
    }

    deleteIGP(id: number) {
        return this.delete(id, this.endPointControllerName + this.iGPTypeEndPoints.deleteIGPType)
            .pipe();
    }
}
