import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TitleSearchServiceService {
  private apiUrl = 'http://localhost:8080/api/v1/titles';  // Update later with an actual api endpoint

  constructor(private http: HttpClient) { }

  searchTitle(query: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}?query=${query}`);
  }
}
