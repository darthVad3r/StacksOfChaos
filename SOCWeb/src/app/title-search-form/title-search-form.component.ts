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

  constructor(
    @Inject(TitleSearchService) private titleSearchService: TitleSearchService
  ) {}

  searchForTitle() {
    alert(this.inputText);
    try {
      this.titleSearchService.searchTitles(this.inputText).subscribe({
        next: (response: { titles: string[] }) => {
          this.titles = response.titles;
        },
        error: (error: HttpErrorResponse) => {
          console.error(
            `Error occurred while searching for "${this.inputText}": `,
            error.message
          );
        },
      });
    } catch (error) {
      console.error('Error occurred while searching for title: ', error);
    }
  }
}
