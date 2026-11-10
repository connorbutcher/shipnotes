import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { firstValueFrom } from 'rxjs';
import { ShowcaseSession } from '../../../core/models/showcase.model';
import { PublicApiConsole } from './public-api-console';
import { DEFAULT_QUERY } from './query-request';

const session: ShowcaseSession = {
  apiKey: 'sn_demo',
  application: {
    id: 'app-1',
    name: 'Orbit',
    iconUrl: null,
    apiKeyPrefix: 'sn_demo',
    createdAt: '',
    updatedAt: '',
  },
};

describe('PublicApiConsole', () => {
  let apiConsole: PublicApiConsole;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    apiConsole = TestBed.inject(PublicApiConsole);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('sends the API key and reports the response', async () => {
    const result = firstValueFrom(apiConsole.send(session, DEFAULT_QUERY));

    const request = http.expectOne('/api/v1/public/applications/app-1/release-notes/latest');
    expect(request.request.headers.get('X-Api-Key')).toBe('sn_demo');
    request.flush({ version: '2.0.0' });

    expect(await result).toMatchObject({ ok: true, status: 200, body: { version: '2.0.0' } });
  });

  it('reports errors as results instead of throwing', async () => {
    const result = firstValueFrom(apiConsole.send(session, { ...DEFAULT_QUERY, sendApiKey: false }));

    const request = http.expectOne('/api/v1/public/applications/app-1/release-notes/latest');
    expect(request.request.headers.has('X-Api-Key')).toBe(false);
    request.flush(null, { status: 401, statusText: 'Unauthorized' });

    expect(await result).toMatchObject({ ok: false, status: 401, statusText: 'Unauthorized' });
  });
});
