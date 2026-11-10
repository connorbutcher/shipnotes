import { ReleaseNoteCategory } from '../../../core/models/release-note-category';

/** The public API endpoints the showcase console can call. */
export type EndpointId = 'list' | 'between' | 'latest' | 'version' | 'application';

/** A query option shown in the builder when the selected endpoint supports it. */
export type QueryField =
  | 'version'
  | 'from'
  | 'to'
  | 'includeFrom'
  | 'after'
  | 'upTo'
  | 'limit'
  | 'includePreReleases'
  | 'category';

/** Everything the console needs to build one public API request. */
export interface QueryRequest {
  endpoint: EndpointId;
  version: string;
  from: string;
  to: string;
  includeFrom: boolean;
  after: string;
  upTo: string;
  limit: number | null;
  includePreReleases: boolean;
  /** Only notes with these categories, and only those sections of them. Empty means all. */
  categories: ReleaseNoteCategory[];
  /** Off sends the request without `X-Api-Key`, to show the 401 response. */
  sendApiKey: boolean;
}

export interface EndpointDefinition {
  id: EndpointId;
  /** Path relative to `/api/v1/public/applications/{applicationId}`. */
  path: string;
  description: string;
  fields: readonly QueryField[];
}

export const ENDPOINTS: readonly EndpointDefinition[] = [
  {
    id: 'latest',
    path: '/release-notes/latest',
    description: 'The newest published note, e.g. for a "What\'s new" badge.',
    fields: ['category', 'includePreReleases'],
  },
  {
    id: 'version',
    path: '/release-notes/{version}',
    description: 'The note for one exact version.',
    fields: ['version', 'category'],
  },
  {
    id: 'between',
    path: '/release-notes/between',
    description:
      'Everything that changed between two versions: after "from" (unless included) up to and including "to".',
    fields: ['from', 'to', 'category', 'includeFrom', 'includePreReleases'],
  },
  {
    id: 'list',
    path: '/release-notes',
    description: 'Published notes, newest first, optionally filtered and limited.',
    fields: ['after', 'upTo', 'limit', 'category', 'includePreReleases'],
  },
  {
    id: 'application',
    path: '/',
    description: 'The application name and icon URL.',
    fields: [],
  },
];

export const DEFAULT_QUERY: QueryRequest = {
  endpoint: 'latest',
  version: '1.1.0',
  from: '1.1.0',
  to: '2.0.0',
  includeFrom: false,
  after: '',
  upTo: '',
  limit: null,
  includePreReleases: false,
  categories: [],
  sendApiKey: true,
};

export function endpointById(id: EndpointId): EndpointDefinition {
  return ENDPOINTS.find((e) => e.id === id) ?? ENDPOINTS[0];
}

/** The public base path for an application. */
export function publicBasePath(applicationId: string): string {
  return `/api/v1/public/applications/${applicationId}`;
}

/**
 * The request path and query string relative to the application's public base path,
 * e.g. `/release-notes/between?from=1.1.0&to=2.0.0`. Only options the endpoint supports
 * are sent, and options left at their API default are left out. Categories are sent
 * comma-separated, e.g. `category=breaking,security`.
 */
export function buildQueryPath(request: QueryRequest): string {
  const endpoint = endpointById(request.endpoint);
  const fields = new Set(endpoint.fields);
  const query = new URLSearchParams();
  const add = (field: QueryField, value: string | number | boolean | null) => {
    if (fields.has(field) && value !== null && value !== '' && value !== false) {
      query.set(field, String(value).trim());
    }
  };

  add('after', request.after);
  add('upTo', request.upTo);
  add('from', request.from);
  add('to', request.to);
  add('includeFrom', request.includeFrom);
  add('limit', request.limit);
  add('includePreReleases', request.includePreReleases);
  if (fields.has('category') && request.categories.length) {
    query.set('category', request.categories.join(','));
  }

  const path =
    endpoint.id === 'version'
      ? endpoint.path.replace('{version}', encodeURIComponent(request.version.trim()))
      : endpoint.path;
  // Category names are plain words, so keep the comma readable instead of %2C.
  const search = query.toString().replace(/%2C/g, ',');

  // The application endpoint is the base path itself, so it has no trailing segment.
  const normalizedPath = path === '/' ? '' : path;
  return search ? `${normalizedPath}?${search}` : normalizedPath;
}
