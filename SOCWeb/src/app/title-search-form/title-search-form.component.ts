import { HttpErrorResponse } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { TitleSearchService } from './title-search-service';

@Component({
  selector: 'app-title-search-form',
  templateUrl: './title-search-form.component.html',
  styleUrl: './title-search-form.component.css'
})
export class TitleSearchFormComponent {
  inputText: string = '';
  titles: string[];

  constructor(@Inject(TitleSearchService) private titleSearchService: TitleSearchService) {
    this.inputText = '';
    this.titles = [];
  }

  onSubmit() {
    console.log("Hello World");
  this.titleSearchService.searchTitles(this.inputText).subscribe({
    next: (response: { titles: string[] }) => {
      this.titles = response.titles;
    },
    error: (error: HttpErrorResponse) => {
      console.error(`Error occurred while searching for "${this.inputText}": `, error.message);
    }
  });
}}