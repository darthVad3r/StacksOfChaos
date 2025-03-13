import { HttpErrorResponse } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { TitleSearchService } from './title-search-service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-title-search-form',
  templateUrl: './title-search-form.component.html',
  styleUrls: ['./title-search-form.component.css'],
  imports: [FormsModule],
})
export class TitleSearchFormComponent {
  inputText: string = '';
  titles: string[] = [];
  titleString: string = '';
  errorMessage: string = '';

  constructor(
    @Inject(TitleSearchService) private titleSearchService: TitleSearchService
  ) {}

  searchForTitle() {
    this.titleSearchService.searchTitles(this.inputText).subscribe({
      next: this.handleSearchSuccess.bind(this),
      error: this.handleSearchError.bind(this),
    });
  }

  handleSearchSuccess(titles: string[]) {
    this.titles = titles;
    this.titleString = titles.toString();
  }

  handleSearchError(error: HttpErrorResponse) {
    console.error(`An error occurred: ${error.message}`);
    this.errorMessage = error.message;
  }
}
