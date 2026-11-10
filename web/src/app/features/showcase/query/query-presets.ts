import { ReleaseNoteCategory } from '../../../core/models/release-note-category';
import { QueryRequest } from './query-request';

/** A ready-made question a consuming app might ask, applied to the console in one click. */
export interface QueryPreset {
  id: string;
  label: string;
  icon: string;
  /** The status this preset is expected to return; shown so error cases are clearly intentional. */
  expectedStatus: number;
  request: Partial<QueryRequest> & Pick<QueryRequest, 'endpoint'>;
}

/** Versions refer to the seeded demo application (see ShowcaseCatalog in the API). */
export const QUERY_PRESETS: readonly QueryPreset[] = [
  {
    id: 'whats-new',
    label: "What's new?",
    icon: 'pi pi-star',
    expectedStatus: 200,
    request: { endpoint: 'latest' },
  },
  {
    id: 'upgrade',
    label: 'Upgraded from 1.1.0 to 2.0.0',
    icon: 'pi pi-arrow-up-right',
    expectedStatus: 200,
    request: { endpoint: 'between', from: '1.1.0', to: '2.0.0', includeFrom: false },
  },
  {
    id: 'one-version',
    label: 'Notes for 1.2.0',
    icon: 'pi pi-tag',
    expectedStatus: 200,
    request: { endpoint: 'version', version: '1.2.0' },
  },
  {
    id: 'with-pre-releases',
    label: 'Include pre-releases',
    icon: 'pi pi-flag',
    expectedStatus: 200,
    request: { endpoint: 'list', includePreReleases: true, after: '', upTo: '', limit: null },
  },
  {
    id: 'last-three',
    label: 'Last 3 releases',
    icon: 'pi pi-list',
    expectedStatus: 200,
    request: { endpoint: 'list', limit: 3, includePreReleases: false, after: '', upTo: '' },
  },
  {
    id: 'security-and-breaking',
    label: 'Only breaking and security changes',
    icon: 'pi pi-shield',
    expectedStatus: 200,
    request: {
      endpoint: 'list',
      categories: ['breaking', 'security'],
      after: '',
      upTo: '',
      limit: null,
    },
  },
  {
    id: 'security-upgrade',
    label: 'Security fixes from 1.0.0 to 2.0.0',
    icon: 'pi pi-lock-open',
    expectedStatus: 200,
    request: { endpoint: 'between', from: '1.0.0', to: '2.0.0', categories: ['security'] },
  },
  {
    id: 'latest-breaking',
    label: 'Latest breaking change',
    icon: 'pi pi-exclamation-triangle',
    expectedStatus: 200,
    request: { endpoint: 'latest', categories: ['breaking'] },
  },
  {
    id: 'app-details',
    label: 'Application details',
    icon: 'pi pi-box',
    expectedStatus: 200,
    request: { endpoint: 'application' },
  },
  {
    id: 'draft',
    label: 'Unpublished draft (404)',
    icon: 'pi pi-eye-slash',
    expectedStatus: 404,
    request: { endpoint: 'version', version: '2.1.0' },
  },
  {
    id: 'no-key',
    label: 'Missing API key (401)',
    icon: 'pi pi-lock',
    expectedStatus: 401,
    request: { endpoint: 'latest', sendApiKey: false },
  },
  {
    id: 'bad-category',
    label: 'Unknown category (400)',
    icon: 'pi pi-question-circle',
    expectedStatus: 400,
    request: { endpoint: 'list', categories: ['bugs' as ReleaseNoteCategory] },
  },
  {
    id: 'bad-range',
    label: 'Backwards range (400)',
    icon: 'pi pi-exclamation-triangle',
    expectedStatus: 400,
    request: { endpoint: 'between', from: '2.0.0', to: '1.0.0' },
  },
];

/** Applies a preset on top of the defaults, so options from an earlier query don't leak in. */
export function applyPreset(base: QueryRequest, preset: QueryPreset): QueryRequest {
  return {
    ...base,
    sendApiKey: true,
    includePreReleases: false,
    categories: [],
    ...preset.request,
  };
}
