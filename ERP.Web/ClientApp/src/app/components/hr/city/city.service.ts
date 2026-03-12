import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { CityEndPoints } from './city.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class CityService extends BaseService<any> {

    endPointControllerName = "City";
    constructor(httpClient: HttpClient, private http: HttpClient, private cityEndPoints: CityEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllCities(citiesFilterForm: any) {
        return this.post(citiesFilterForm, this.endPointControllerName + this.cityEndPoints.getAllCities)
            .pipe(map((data: any) => data));
    }

    saveCity(saveCityCommand: any) {
        return this.post(saveCityCommand, this.endPointControllerName + this.cityEndPoints.saveCity)
            .pipe(map((data: any) => data));
    }

    deleteCity(id: number) {
        return this.delete(id, this.endPointControllerName + this.cityEndPoints.deleteCity)
            .pipe();
    }

    getCityById(id: number) {
        return this.get(id, this.endPointControllerName + this.cityEndPoints.getCityById)
            .pipe(map((data: any) => data));
    }

    getCityByName(name: string) {
        return this.get(name, this.endPointControllerName + this.cityEndPoints.getCityByName)
            .pipe(map((data: any) => data));
    }
}
