import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { NotificationEndPoints } from './notification.endpoints';
import { BaseService } from '../../../Service/base.service';
import { environment } from '../../../../environments/environment';

@Injectable({
    providedIn: 'root'
})

export class NotificationService extends BaseService<any> {

    endPointControllerName = "Notification";
    
    constructor(httpClient: HttpClient, private http: HttpClient, private notificationEndPoints: NotificationEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllNotifications(filterRequest: any) {
        return this.post(filterRequest, this.endPointControllerName + this.notificationEndPoints.getAllNotifications)
            .pipe(map((data: any) => data));
    }

    async getNotificationById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.notificationEndPoints.getNotificationById)
            .pipe(map((data: any) => data));
    }

    saveNotification(notification: any) {
        return this.post(notification, this.endPointControllerName + this.notificationEndPoints.saveNotification)
            .pipe(map((data: any) => data));
    }

    deleteNotification(id: number) {
        return this.delete(id, this.endPointControllerName + this.notificationEndPoints.deleteNotification)
            .pipe();
    }

    async getEmployeeNotifications() {
        return this.get('', this.endPointControllerName + this.notificationEndPoints.getEmployeeNotifications)
            .pipe(map((data: any) => data));
    }
}
