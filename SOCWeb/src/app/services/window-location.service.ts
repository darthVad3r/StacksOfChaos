// implementation of the WindowLocationService
import { Injectable } from '@angular/core';
@Injectable({
  providedIn: 'root',
})
export class WindowLocationService {
  getLocation(): Location {
    return window.location;
  }

  getHash(): string {
    return window.location.hash;
  }

  getSearch(): string {
    return window.location.search;
  }
}
// This service provides methods to access the window location, hash, and search parameters.
// It can be used in components or services that need to interact with the URL without directly accessing the global window object.
// This is useful for testing purposes, as it allows you to mock the location in unit tests.
//     });
//
//     it('should navigate to login if no token is found', () => {