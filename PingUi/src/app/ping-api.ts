import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PingApi {
  http = inject(HttpClient);
  readonly apiUrl = environment.apiUrl;
  
  getPings() {

  }

}
