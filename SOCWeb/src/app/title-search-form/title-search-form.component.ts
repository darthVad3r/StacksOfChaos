import { Component } from '@angular/core';

@Component({
  selector: 'app-title-search-form',
  standalone: false,
  templateUrl: './title-search-form.component.html',
  styleUrl: './title-search-form.component.css'
})
export class TitleSearchFormComponent {
  inputText: string = '';

  onSubmit() {
    console.log('Search for: ' + this.inputText);
  }
}
