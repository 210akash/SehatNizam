import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../../Service/base.service';
import { environment } from '../../../../environments/environment';
import { WardEndPoints } from './ward.endpoints';

@Injectable({
    providedIn: 'root'
})

export class WardService extends BaseService<any> {

    endPointControllerName = "Ward";
    constructor(httpClient: HttpClient, private http: HttpClient, private wardEndPoints: WardEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllWards(wardsFilterForm: any) {
        return this.post(wardsFilterForm, this.endPointControllerName + this.wardEndPoints.getAllWards)
            .pipe(map((data: any) => data));
    }

    saveWard(saveWardCommand: any) {
        return this.post(saveWardCommand, this.endPointControllerName + this.wardEndPoints.saveWard)
            .pipe(map((data: any) => data));
    }

    deleteWard(id: number) {
        return this.delete(id, this.endPointControllerName + this.wardEndPoints.deleteWard)
            .pipe();
    }

    getWardCode() {
        return this.get(this.endPointControllerName + this.wardEndPoints.getWardCode)
            .pipe(map((data: any) => data));
    }
}
