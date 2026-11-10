import { InjectionToken } from '@angular/core';
import { environment } from '../../../environments/environment';

/** Base URL that API paths are appended to. */
export const API_BASE_URL = new InjectionToken<string>('API_BASE_URL', {
  providedIn: 'root',
  factory: () => environment.apiBaseUrl,
});
