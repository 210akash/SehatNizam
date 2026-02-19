import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { UomEndPoints } from './uom.endpoints';

@Injectable({
    providedIn: 'root'
})

export class UomService extends BaseService<any> {

    endPointControllerName = "UOM";
    constructor(httpClient: HttpClient, private http: HttpClient, private uomEndPoints: UomEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllUoms(uomsFilterForm: any) {
        return this.post(uomsFilterForm, this.endPointControllerName + this.uomEndPoints.getAllUoms)
            .pipe(map((data: any) => data));
    }

    saveUom(saveUomCommand: any) {
        return this.post(saveUomCommand, this.endPointControllerName + this.uomEndPoints.saveUom)
            .pipe(map((data: any) => data));
    }

    deleteUom(id: number) {
        return this.delete(id, this.endPointControllerName + this.uomEndPoints.deleteUom)
            .pipe();
    }

    getUomById(id: number) {
        return this.get(id, this.endPointControllerName + this.uomEndPoints.getUomById)
            .pipe(map((data: any) => data));
    }

    getUomByName(name: string) {
        return this.get(name, this.endPointControllerName + this.uomEndPoints.getUomByName)
            .pipe(map((data: any) => data));
    }

    GetUOMByCompany(companyId: number) {
        return this.get('?CompanyId=' + companyId, this.endPointControllerName + this.uomEndPoints.getUOMByCompany)
            .pipe(map((data: any) => data));
    }
}
