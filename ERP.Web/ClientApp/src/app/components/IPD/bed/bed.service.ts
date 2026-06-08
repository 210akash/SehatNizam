import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';
import { BedEndPoints } from './bed.endpoints';

@Injectable({
    providedIn: 'root'
})

export class BedService extends BaseService<any> {

    endPointControllerName = "Bed";
    constructor(httpClient: HttpClient, private http: HttpClient, private bedEndPoints: BedEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllBeds(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.bedEndPoints.getAllBeds)
            .pipe(map((data: any) => data));
    }

    saveBed(saveBedCommand: any) {
        return this.post(saveBedCommand, this.endPointControllerName + this.bedEndPoints.saveBed)
            .pipe(map((data: any) => data));
    }

    deleteBed(id: number) {
        return this.delete(id, this.endPointControllerName + this.bedEndPoints.deleteBed)
            .pipe();
    }

    getBedByName(name: string) {
        return this.get(name, this.endPointControllerName + this.bedEndPoints.getBedByName)
            .pipe(map((data: any) => data));
    }

    getBedCode(RoomId:number,Id:number) {
        return this.get('?roomId=' + RoomId + '&Id=' + Id , this.endPointControllerName + this.bedEndPoints.getBedCode)
            .pipe(map((data: any) => data));
    }

    getBedByRoom(RoomId:number) {
        return this.get('?roomId=' + RoomId, this.endPointControllerName + this.bedEndPoints.getBedByRoom)
            .pipe(map((data: any) => data));
    }
}
