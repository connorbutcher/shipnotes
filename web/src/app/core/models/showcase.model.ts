import { Application } from './application.model';

/** The demo application and a freshly issued API key for it. */
export interface ShowcaseSession {
  application: Application;
  apiKey: string;
}
