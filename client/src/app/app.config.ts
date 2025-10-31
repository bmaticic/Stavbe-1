import { ApplicationConfig, importProvidersFrom, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { routes } from './app.routes';
import { provideToastr } from 'ngx-toastr';
import { errorInterceptor } from './_interceptors/error.interceptor';
import { jwtInterceptor } from './_interceptors/jwt.interceptor';
import { NgxSpinnerModule } from 'ngx-spinner';
import { loadingInterceptor } from './_interceptors/loading.interceptor';
import { ModalModule, BsModalService } from 'ngx-bootstrap/modal';
import { BsDatepickerModule, BsLocaleService } from 'ngx-bootstrap/datepicker';

import {  LOCALE_ID } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import localeSi from '@angular/common/locales/sl';
registerLocaleData(localeSi);

import { defineLocale } from 'ngx-bootstrap/chronos';
import { slLocale } from 'ngx-bootstrap/locale';
defineLocale('sl', slLocale);




export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }), 
    provideRouter(routes),
    provideHttpClient(withInterceptors([errorInterceptor, jwtInterceptor, loadingInterceptor])),
    provideAnimations(),
    provideToastr({ positionClass: 'toast-bottom-right' }),
    importProvidersFrom(NgxSpinnerModule, BsDatepickerModule.forRoot(), ModalModule.forRoot(), ReactiveFormsModule),
    { provide: LOCALE_ID, useValue: 'sl' },
  ]
};
