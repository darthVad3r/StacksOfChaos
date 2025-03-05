import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TitleSearchService {
  private apiUrl = 'https://localhost:5001/api/title'; // Update with the actual API endpoint

  constructor(private https: HttpClient) { }

  searchTitles(query: string): Observable<any> {
    return this.https.get<any>(`${this.apiUrl}?searchString=${query}`);
  }
}