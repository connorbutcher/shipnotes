import { DEFAULT_QUERY, QueryRequest, buildQueryPath } from './query-request';

const query = (changes: Partial<QueryRequest>): QueryRequest => ({ ...DEFAULT_QUERY, ...changes });

describe('buildQueryPath', () => {
  it('builds the latest path with only the options that endpoint accepts', () => {
    expect(buildQueryPath(query({ endpoint: 'latest', from: '1.0.0' }))).toBe(
      '/release-notes/latest',
    );
    expect(buildQueryPath(query({ endpoint: 'latest', includePreReleases: true }))).toBe(
      '/release-notes/latest?includePreReleases=true',
    );
  });

  it('puts the version in the path, encoded', () => {
    expect(buildQueryPath(query({ endpoint: 'version', version: ' 2.0.0-rc.1+build.5 ' }))).toBe(
      '/release-notes/2.0.0-rc.1%2Bbuild.5',
    );
  });

  it('builds a between query', () => {
    expect(
      buildQueryPath(query({ endpoint: 'between', from: '1.1.0', to: '2.0.0', includeFrom: true })),
    ).toBe('/release-notes/between?from=1.1.0&to=2.0.0&includeFrom=true');
  });

  it('leaves out empty list options', () => {
    expect(buildQueryPath(query({ endpoint: 'list', after: '', upTo: '', limit: null }))).toBe(
      '/release-notes',
    );
    expect(buildQueryPath(query({ endpoint: 'list', after: '1.0.0', limit: 3 }))).toBe(
      '/release-notes?after=1.0.0&limit=3',
    );
  });

  it('uses the base path itself for application details', () => {
    expect(buildQueryPath(query({ endpoint: 'application' }))).toBe('');
  });

  it('sends categories comma-separated, only where supported', () => {
    expect(
      buildQueryPath(query({ endpoint: 'latest', categories: ['breaking', 'security'] })),
    ).toBe('/release-notes/latest?category=breaking,security');
    expect(buildQueryPath(query({ endpoint: 'application', categories: ['fixed'] }))).toBe('');
  });
});
