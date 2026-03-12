import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { HRYearEndPoints } from './hryear.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class HRYearService extends BaseService<any> {

    endPointControllerName = "HRYear";
    constructor(httpClient: HttpClient, private http: HttpClient, private hryearEndPoints: HRYearEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllHryear(citiesFilterForm: any) {
        return this.post(citiesFilterForm, this.endPointControllerName + this.hryearEndPoints.getAllHryear)
            .pipe(map((data: any) => data));
    }

    saveHRYear(saveHRYearCommand: any) {
        return this.post(saveHRYearCommand, this.endPointControllerName + this.hryearEndPoints.saveHRYear)
            .pipe(map((data: any) => data));
    }

    deleteHRYear(id: number) {
        return this.delete(id, this.endPointControllerName + this.hryearEndPoints.deleteHRYear)
            .pipe();
    }

    getHRYearById(id: number) {
        return this.get(id, this.endPointControllerName + this.hryearEndPoints.getHRYearById)
            .pipe(map((data: any) => data));
    }
}
