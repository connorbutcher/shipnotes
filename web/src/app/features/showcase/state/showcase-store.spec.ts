import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { ApplicationsStore } from '../../../core/state/applications-store';
import { ShowcaseStore } from './showcase-store';

const session = {
  apiKey: 'sn_demo',
  application: {
    id: 'demo',
    name: 'Orbit Tasks (demo)',
    iconUrl: null,
    apiKeyPrefix: 'sn_demo',
    createdAt: '',
    updatedAt: '',
  },
};

describe('ShowcaseStore', () => {
  let store: ShowcaseStore;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting(), ShowcaseStore],
    });
    store = TestBed.inject(ShowcaseStore);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('starts a session and adds the demo to the side panel', () => {
    store.start();
    expect(store.status()).toBe('loading');

    http.expectOne({ method: 'POST', url: '/api/v1/showcase/session' }).flush(session);

    expect(store.status()).toBe('ready');
    expect(store.session()?.apiKey).toBe('sn_demo');
    expect(TestBed.inject(ApplicationsStore).byId('demo')?.name).toBe('Orbit Tasks (demo)');
  });

  it('reports a disabled showcase', () => {
    store.start();

    http
      .expectOne('/api/v1/showcase/session')
      .flush({ title: 'Not found' }, { status: 404, statusText: 'Not Found' });

    expect(store.status()).toBe('disabled');
  });

  it('bumps the revision when the demo is reset', () => {
    store.reset().subscribe();
    http.expectOne({ method: 'POST', url: '/api/v1/showcase/reset' }).flush(session);

    expect(store.revision()).toBe(1);
    expect(store.resetting()).toBe(false);
  });
});
