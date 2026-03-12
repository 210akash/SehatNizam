import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { RejectReasonEndPoints } from './rejectreason.endpoints';

@Injectable({
    providedIn: 'root'
})

export class RejectReasonService extends BaseService<any> {

    endPointControllerName = "RejectReason";
    constructor(httpClient: HttpClient, private http: HttpClient, private RejectReasonEndPoints: RejectReasonEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllRejectReasons(rejectreasonsFilterForm: any) {
        return this.post(rejectreasonsFilterForm, this.endPointControllerName + this.RejectReasonEndPoints.getAllRejectReasons)
            .pipe(map((data: any) => data));
    }

    saveRejectReason(saveRejectReasonCommand: any) {
        return this.post(saveRejectReasonCommand, this.endPointControllerName + this.RejectReasonEndPoints.saveRejectReason)
            .pipe(map((data: any) => data));
    }

    deleteRejectReason(id: number) {
        return this.delete(id, this.endPointControllerName + this.RejectReasonEndPoints.deleteRejectReason)
            .pipe();
    }
}
