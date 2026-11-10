import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withComponentInputBinding, withRouterConfig } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { routes } from './app.routes';
import { errorNotificationInterceptor } from './core/http/error-notification.interceptor';
import { provideShipNotesPrimeNG } from './core/theme/primeng.config';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(
      routes,
      withComponentInputBinding(),
      // Child routes receive parent params (e.g. applicationId) as component inputs.
      withRouterConfig({ paramsInheritanceStrategy: 'always' }),
    ),
    provideHttpClient(withFetch(), withInterceptors([errorNotificationInterceptor])),
    provideShipNotesPrimeNG(),
    MessageService,
    ConfirmationService,
  ],
};
