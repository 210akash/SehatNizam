import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SurgicalOrderEndPoints {
  getAllSurgicalOrders = '/GetAllSurgicalOrders';
  saveSurgicalOrder = '/SaveSurgicalOrder';
  deleteSurgicalOrder = '/DeleteSurgicalOrder';
}
