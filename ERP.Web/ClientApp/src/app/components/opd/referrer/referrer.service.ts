import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';
import { ReferrerEndPoints } from './referrer.endpoints';

@Injectable({
    providedIn: 'root'
})

export class ReferrerService extends BaseService<any> {

    endPointControllerName = "Referrer";
    constructor(httpClient: HttpClient, private http: HttpClient, private ReferrerEndPoints: ReferrerEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllReferrers(query: { departmentId?: number }) {
        return this.post(query, this.endPointControllerName + this.ReferrerEndPoints.getAllReferrer)
            .pipe(map((data: any) => data));
    }

    saveReferrer(command: { id?: number; code: string; name: string; basePrice: number; departmentId?: number | null }) {
        return this.post(command, this.endPointControllerName + this.ReferrerEndPoints.saveReferrer)
            .pipe(map((data: any) => data));
    }

    deleteReferrer(id: number) {
        return this.delete(id, this.endPointControllerName + this.ReferrerEndPoints.deleteReferrer)
            .pipe();
    }

    getReferrerByName(name: string) {
        return this.get('?name='+name, this.endPointControllerName + this.ReferrerEndPoints.getReferrerByName)
            .pipe(map((data: any) => data));
    }

}
