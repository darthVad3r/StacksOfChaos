// create a unit test for the WindowLocationService
import { TestBed } from '@angular/core/testing';
import { WindowLocationService } from './window-location.service';
import { Location } from '@angular/common';
import { getTestBed } from '@angular/core/testing';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AuthService } from './auth.service';
import { Router } from '@angular/router';
import { of } from 'rxjs';

@NgModule({
  imports: [
    BrowserModule
  ],
  providers: [AuthService, WindowLocationService]
})
export class AppModule {}
describe('WindowLocationService', () => {
  let service: WindowLocationService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [AppModule],
      providers: [WindowLocationService]
    });
    service = TestBed.inject(WindowLocationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should return the current location', () => {
    const location: Location = service.getLocation() as unknown as Location;
    expect(location).toBe(window.location as unknown as Location);
  });

  it('should return the current hash', () => {
    const hash: string = service.getHash();
    expect(hash).toBe(window.location.hash);
  });

  it('should return the current search parameters', () => {
    const search: string = service.getSearch();
    expect(search).toBe(window.location.search);
  });
});