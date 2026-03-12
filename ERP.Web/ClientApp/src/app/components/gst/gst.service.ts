import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { GSTEndPoints } from './gst.endpoints';

@Injectable({
    providedIn: 'root'
})

export class GSTService extends BaseService<any> {

    endPointControllerName = "GST";
    constructor(httpClient: HttpClient, private http: HttpClient, private gstEndPoints: GSTEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllGST(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.gstEndPoints.getAllGST)
            .pipe(map((data: any) => data));
    }

    saveGST(saveGSTCommand: any) {
        return this.post(saveGSTCommand, this.endPointControllerName + this.gstEndPoints.saveGST)
            .pipe(map((data: any) => data));
    }

    deleteGST(id: number) {
        return this.delete(id, this.endPointControllerName + this.gstEndPoints.deleteGST)
            .pipe();
    }

    getGSTById(id: number) {
        return this.get(id, this.endPointControllerName + this.gstEndPoints.getGSTById)
            .pipe(map((data: any) => data));
    }

    getGSTByName(name: string) {
        return this.get(name, this.endPointControllerName + this.gstEndPoints.getGSTByName)
            .pipe(map((data: any) => data));
    }

    getCurrentGST() {
        return this.get(this.endPointControllerName + this.gstEndPoints.getCurrentGST)
            .pipe(map((data: any) => data));
    }
}
