import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TitleSearchService {
  private apiUrl = 'http://localhost:5073/api/titlesearch'; // Update with the actual API endpoint

  constructor(private http: HttpClient) { }

  searchTitles(query: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}?query=${query}`);
  }
}