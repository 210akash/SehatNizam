import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

export class BaseService<T> {


  constructor(private httpClient: HttpClient,private url: string) 
  {

  }

  public post(item: T, endpoint: string = ''): Observable<T> {
    return this.httpClient
      .post<T>(`${this.url}/${endpoint}`, item)
      .pipe(map((data: any) => data));
  }

  public put(item: T, id: any, endpoint: string = ''): Observable<T> {
    if (id === '' || id == null) {
      return this.httpClient
        .put<T>(`${this.url}/${endpoint}`, item)
        .pipe(map((data: any) => data as T));
    } else if (endpoint !== '' && endpoint != null) {
      return this.httpClient
        .put<T>(`${this.url}/${endpoint}/${id}`, item)
        .pipe(map((data: any) => data as T));
    } else {
      return this.httpClient
        .put<T>(`${this.url}/${id}`, item)
        .pipe(map((data: any) => data as T));
    }
  }

  public putExternal(item: T, id: any, endpoint: string = ''): Observable<T> {
   // return this.httpClient.get<T>(`${this.url}/${endpoint}${id}`, {data:item}).pipe(map((data: any) => data as T));
    return this.httpClient.post<T>(`${this.url}/${endpoint}${id}`,{data:item}).pipe(map((data: any) => data as T));
      // return this.httpClient
      //   .get<T>(`${this.url}/${endpoint}${id}`, item)
      //   .pipe(map((data: any) => data as T));
  }

  getexternal(id: any, endpoint: string = '', resType?:any): Observable<T> {
      return this.httpClient
        .get(`${this.url}/${endpoint}${id}`)
        .pipe(map((data: any) => data as T));
  }
  getexternalk(url: any): Observable<T> {
    const header = new HttpHeaders()
    header.set("Access-Control-Allow-Credentials",   "true");
    header.set("Access-Control-Allow-Headers",  "X-PINGOTHER, Content-Type,Authorization");
      header.set("Access-Control-Allow-Methods" ,"GET, POST, PUT, PATCH, DELETE, HEAD, OPTIONS");
        header.set("Access-Control-Allow-Origin", "http://localhost:4200");
     //   header.set("cookies", "username=cookies");
        header.set("Access-Control-Allow-Credentials", "true")
      return this.httpClient
        .get(`${url}`,{headers:header})
        .pipe(map((data: any) => data as T));
}
getexternalwithcookies1k(id: any, endpoint: string = '', resType?:any): Observable<T> {
  const header = new HttpHeaders()
header.append("Access-Control-Allow-Origin", "http://localhost:4200");
header.append("Access-Control-Allow-Methods", "DELETE, POST, GET, OPTIONS");
header.append("cookies", "username=cookies");
header.append("Access-Control-Allow-Headers", "Content-Type, Authorization, X-Requested-With");
header.append("Access-Control-Allow-Credentials", "true");

  return this.httpClient
    .get(`${this.url}/${endpoint}${id}`,{headers:header})
    .pipe(map((data: any) => data as T));
}
  getexternal2(URL: string = ''): Observable<T> {
    const header = new HttpHeaders()
    header.append("Access-Control-Allow-Origin", "*");
    header.append("Access-Control-Allow-Methods", "DELETE, POST, GET, OPTIONS");
    header.append("cookies", "username=cookies");
    header.append("Access-Control-Allow-Headers", "Content-Type, Authorization, X-Requested-With");
    header.append("Access-Control-Allow-Credentials", "true");

    return this.httpClient
      .get(`${this.url}`,{headers:header})
      .pipe(map((data: any) => data as T));
}
  getexternalwithcookies1(id: any, endpoint: string = '', resType?:any): Observable<T> {
    const header = new HttpHeaders()
header.append("Access-Control-Allow-Origin", "http://localhost:4200");
header.append("Access-Control-Allow-Methods", "DELETE, POST, GET, OPTIONS");
header.append("cookies", "username=cookies");
header.append("Access-Control-Allow-Headers", "Content-Type, Authorization, X-Requested-With");
header.append("Access-Control-Allow-Credentials", "true");

    return this.httpClient
      .get(`${this.url}/${endpoint}${id}`,{headers:header})
      .pipe(map((data: any) => data as T));
}

  getexternalwithcookies(id: any, endpoint: string = '', resType?:any): Observable<T> {
    const header = new HttpHeaders()
//     .append('Cookie', 'username')
//     .append('Access-Control-Allow-Headers', 'Content-Type, Authorization');
//     // .append('Access-Control-Allow-Origin', 'https://localhost:8083')
//     // .append('Content-Type', 'application/x-www-form-urlencoded')
// console.log(header);

// header.append("Access-Control-Allow-Origin", "*");
// header.append("Access-Control-Allow-Methods", "DELETE, POST, GET, OPTIONS");
// header.append("cookies", "username=cookies");
// header.append("Access-Control-Allow-Headers", "Content-Type, Authorization, X-Requested-With");
// header.set('Content-Type', 'application/json; charset=utf-8');
//  header.append("Access-Control-Allow-Credentials", "true");


 header.set("Access-Control-Allow-Credentials",   "true");
  header.set("Access-Control-Allow-Headers",  "X-PINGOTHER, Content-Type");
    header.set("Access-Control-Allow-Methods" ,"GET, POST, PUT, PATCH, DELETE, HEAD, OPTIONS");
      header.set("Access-Control-Allow-Origin", "http://localhost:4200");
      header.set("cookies", "username=cookies");
      header.set("Access-Control-Allow-Credentials", "true")

    return this.httpClient
      .get(`${this.url}/${endpoint}${id}`,{headers:header})
      .pipe(map((data: any) => data as T));
}

  get(id: any, endpoint: string = '', resType?:any): Observable<T> {
    if (resType) {
      return this.httpClient
        .get(`${this.url}/${id}`, { responseType: resType })
        .pipe(map((data: any) => data as T));
    }
    else if (endpoint) {
      return this.httpClient
        .get(`${this.url}/${endpoint}/${id}`)
        .pipe(map((data: any) => data as T));

    } else if (id) {
      return this.httpClient
        .get(`${this.url}/${id}`)
        .pipe(map((data: any) => data as T));
    } else {
      return this.httpClient
        .get(`${this.url}`)
        .pipe(map((data: any) => data as T));
    }
  }

  getString(id: any, endpoint: string = '', resType?:any): Observable<T> {
    return this.httpClient
        .get(`${this.url}/${endpoint}/${id}`, { responseType: resType })
        .pipe(map((data: any) => data as T));
  }

  delete(id: number, endpoint: string = '', options?: any) {
    if (options) {
      return this.httpClient
        .delete(`${this.url}/${endpoint}`, options);
    }
    else if (endpoint !== '' && endpoint != null && id > 0) {
      return this.httpClient
        .delete(`${this.url}/${endpoint}?id=${id}`);
    } else if (id > 0) {
      return this.httpClient
        .delete(`${this.url}?id=${id}`)
        .pipe(map((data: any) => data as T));
    } else {
      return this.httpClient
      .delete(`${this.url}/${endpoint}`);
    }
  }

}
