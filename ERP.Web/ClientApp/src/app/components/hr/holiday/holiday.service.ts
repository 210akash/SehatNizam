import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { HolidayEndPoints } from './holiday.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class HolidayService extends BaseService<any> {

    endPointControllerName = "Holiday";
    constructor(httpClient: HttpClient, private http: HttpClient, private holidayEndPoints: HolidayEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllHryear(citiesFilterForm: any) {
        return this.post(citiesFilterForm, this.endPointControllerName + this.holidayEndPoints.getAllHoliday)
            .pipe(map((data: any) => data));
    }

    saveHoliday(saveHolidayCommand: any) {
        return this.post(saveHolidayCommand, this.endPointControllerName + this.holidayEndPoints.saveHoliday)
            .pipe(map((data: any) => data));
    }

    deleteHoliday(id: number) {
        return this.delete(id, this.endPointControllerName + this.holidayEndPoints.deleteHoliday)
            .pipe();
    }
}
