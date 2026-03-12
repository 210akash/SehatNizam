import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { LocationEndPoints } from './location.endpoints';

@Injectable({
    providedIn: 'root'
})

export class LocationService extends BaseService<any> {

    endPointControllerName = "Location";
    constructor(httpClient: HttpClient, private http: HttpClient, private locationEndPoints: LocationEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllLocations(locationsFilterForm: any) {
        return this.post(locationsFilterForm, this.endPointControllerName + this.locationEndPoints.getAllLocations)
            .pipe(map((data: any) => data));
    }

    saveLocation(saveLocationCommand: any) {
        return this.post(saveLocationCommand, this.endPointControllerName + this.locationEndPoints.saveLocation)
            .pipe(map((data: any) => data));
    }

    deleteLocation(id: number) {
        return this.delete(id, this.endPointControllerName + this.locationEndPoints.deleteLocation)
            .pipe();
    }

    getLocationById(id: number) {
        return this.get(id, this.endPointControllerName + this.locationEndPoints.getLocationById)
            .pipe(map((data: any) => data));
    }

    getLocationByName(name: string) {
        return this.get(name, this.endPointControllerName + this.locationEndPoints.getLocationByName)
            .pipe(map((data: any) => data));
    }

    getLocationByCompany(companyId: string) {
        return this.get('?CompanyId=' + companyId, this.endPointControllerName + this.locationEndPoints.getLocationByCompany)
            .pipe(map((data: any) => data));
    }
}
