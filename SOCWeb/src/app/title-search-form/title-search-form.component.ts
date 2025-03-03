import { Component } from '@angular/core';
import { TitleSearchService } from './title-search-service.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-title-search-form',
  templateUrl: './title-search-form.component.html',
  styleUrl: './title-search-form.component.css'
})
export class TitleSearchFormComponent {
  inputText: string = '';
  titles: string[] = [];

  constructor(private titleSearchService: TitleSearchService) { }

  onSubmit() {
  this.titleSearchService.searchTitles(this.inputText).subscribe({
    next: (response: { titles: string[] }) => {
      this.titles = response.titles;
    },
    error: (error: HttpErrorResponse) => {
      console.log('Error: ', error.message);
    }
  });
}}