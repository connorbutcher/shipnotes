import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { Application } from '../models/application.model';
import { ApplicationsStore } from './applications-store';

const app = (id: string, name: string): Application => ({
  id,
  name,
  iconUrl: null,
  apiKeyPrefix: 'sn_abc',
  createdAt: '',
  updatedAt: '',
});

describe('ApplicationsStore', () => {
  let store: ApplicationsStore;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    store = TestBed.inject(ApplicationsStore);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('loads applications', () => {
    store.load();
    expect(store.loading()).toBe(true);

    http.expectOne('/api/v1/applications').flush([app('1', 'Portal')]);

    expect(store.loading()).toBe(false);
    expect(store.applications().map((a) => a.name)).toEqual(['Portal']);
  });

  it('adds created applications in name order and remembers the issued key', () => {
    store.load();
    http.expectOne('/api/v1/applications').flush([app('1', 'Zeta')]);

    store.create({ name: 'Alpha', iconUrl: null }).subscribe();
    http
      .expectOne({ method: 'POST', url: '/api/v1/applications' })
      .flush({ application: app('2', 'Alpha'), apiKey: 'sn_secret' });

    expect(store.applications().map((a) => a.name)).toEqual(['Alpha', 'Zeta']);
    expect(store.lastIssuedKey()).toEqual({ applicationId: '2', apiKey: 'sn_secret' });

    store.clearIssuedKey();
    expect(store.lastIssuedKey()).toBeNull();
  });

  it('removes deleted applications', () => {
    store.load();
    http.expectOne('/api/v1/applications').flush([app('1', 'Portal')]);

    store.delete('1').subscribe();
    http.expectOne({ method: 'DELETE', url: '/api/v1/applications/1' }).flush(null);

    expect(store.applications()).toEqual([]);
  });
});
