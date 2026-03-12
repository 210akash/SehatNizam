import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BaseService } from '../../Service/base.service';
import { RowEndPoints } from './row.endpoints';

@Injectable({
    providedIn: 'root'
})

export class RowService extends BaseService<any> {

    endPointControllerName = 'Row';

    constructor(private http: HttpClient, httpClient: HttpClient, private rowEndPoints: RowEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getRowByName(name: string) {
        return await this.get('?name=' + name, this.endPointControllerName + this.rowEndPoints.getRowByName)
            .pipe(map((data: any) => data));
    }

    async saveRow(createRowForm: any) {
        return await this.post(createRowForm, this.endPointControllerName + this.rowEndPoints.saveRow)
            .pipe(map((data: any) => data));
    }

    async getAllRow(rowFilterForm: any) {
        return await this.post(rowFilterForm, this.endPointControllerName + this.rowEndPoints.getAllRow)
            .pipe(map((data: any) => data));
    }

    async getRowByRackId(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.rowEndPoints.getRowByRackId)
            .pipe(map((data: any) => data));
    }

    async deleteRow(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.rowEndPoints.deleteRow)
            .pipe(map((data: any) => data));
    }
}
