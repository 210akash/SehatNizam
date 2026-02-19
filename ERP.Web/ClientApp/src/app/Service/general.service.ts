import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from './base.service';
import { GeneralEndPoints } from './general.endpoints';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class GeneralService extends BaseService<any>{

    endPointControllerName = "General";
    datePipe: any;
    Date: any;
    constructor(httpClient: HttpClient, private http: HttpClient, private generalEndPoints: GeneralEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }


    getAllSelectWhereData(table: string, valueColumn: string, textColumn: string, where: string, parm: string, howmany?: number) {
        if (howmany != null)
            return this.get(this.endPointControllerName + this.generalEndPoints.getSelectDataWhere + "?Table=" + table + "&ValueColumn=" + valueColumn + "&TextColumn=" + textColumn + "&Where=" + where + "&Parm=" + parm + "&howmany=" + howmany).pipe(map((data: any) => data));
        else
            return this.get(this.endPointControllerName + this.generalEndPoints.getSelectDataWhere + "?Table=" + table + "&ValueColumn=" + valueColumn + "&TextColumn=" + textColumn + "&Where=" + where + "&Parm=" + parm).pipe(map((data: any) => data));
    }

    getAllMemberType() {
        debugger
        return this.get(this.endPointControllerName + this.generalEndPoints.getAllMemberType)
            .pipe(map((data: any) => data));
    }





}
