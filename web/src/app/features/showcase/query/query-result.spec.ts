import { QueryResult, interpretResult, reasonPhrase, statusSeverity } from './query-result';

const note = {
  version: '1.0.0',
  isPreRelease: false,
  title: 'One',
  summaryHtml: '<p>x</p>',
  sections: [],
  contentHtml: '<p>x</p>',
  publishedAt: '',
};
const result = (changes: Partial<QueryResult>): QueryResult => ({
  method: 'GET',
  url: '/x',
  status: 200,
  statusText: 'OK',
  ok: true,
  durationMs: 1,
  body: null,
  ...changes,
});

describe('interpretResult', () => {
  it('shows lists and single notes as notes', () => {
    expect(interpretResult(result({ body: [note] }))).toEqual({ kind: 'notes', notes: [note] });
    expect(interpretResult(result({ body: note }))).toEqual({ kind: 'notes', notes: [note] });
  });

  it('shows an empty list as empty', () => {
    expect(interpretResult(result({ body: [] }))).toEqual({ kind: 'empty' });
  });

  it('shows application details', () => {
    const application = { name: 'Orbit', iconUrl: null };
    expect(interpretResult(result({ body: application }))).toEqual({
      kind: 'application',
      application,
    });
  });

  it('explains errors from problem details, preferring field errors', () => {
    const problem = {
      title: 'Validation failed',
      errors: { from: ['"from" must not be later than "to".'] },
    };
    expect(interpretResult(result({ ok: false, status: 400, body: problem }))).toEqual({
      kind: 'problem',
      message: '"from" must not be later than "to".',
    });
    expect(
      interpretResult(result({ ok: false, status: 401, statusText: 'Unauthorized', body: null })),
    ).toEqual({ kind: 'problem', message: 'Unauthorized' });
  });
});

describe('status helpers', () => {
  it('maps statuses to severities and reason phrases', () => {
    expect(statusSeverity(200)).toBe('success');
    expect(statusSeverity(404)).toBe('warn');
    expect(statusSeverity(0)).toBe('danger');
    expect(reasonPhrase(403)).toBe('Forbidden');
  });
});
