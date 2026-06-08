import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { RoomEndPoints } from './room.endpoints';
import { BaseService } from '../../../Service/base.service';
import { environment } from '../../../../environments/environment';

@Injectable({
    providedIn: 'root'
})

export class RoomService extends BaseService<any> {

    endPointControllerName = "Room";
    constructor(httpClient: HttpClient, private http: HttpClient, private RoomEndPoints: RoomEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllRooms(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.RoomEndPoints.getAllRooms)
            .pipe(map((data: any) => data));
    }

    saveRoom(saveRoomCommand: any) {
        return this.post(saveRoomCommand, this.endPointControllerName + this.RoomEndPoints.saveRoom)
            .pipe(map((data: any) => data));
    }

    deleteRoom(id: number) {
        return this.delete(id, this.endPointControllerName + this.RoomEndPoints.deleteRoom)
            .pipe();
    }

    getRoomCode(WardId:number,Id:number) {
        return this.get('?WardId=' + WardId + '&Id=' + Id , this.endPointControllerName + this.RoomEndPoints.getRoomCode)
            .pipe(map((data: any) => data));
    }

    getRoomByWard(WardId:number) {
        return this.get('?WardId=' + WardId , this.endPointControllerName + this.RoomEndPoints.getRoomByWard)
            .pipe(map((data: any) => data));
    }
}
