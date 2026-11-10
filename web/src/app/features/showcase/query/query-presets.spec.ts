import { QUERY_PRESETS, applyPreset } from './query-presets';
import { DEFAULT_QUERY } from './query-request';

describe('applyPreset', () => {
  it('starts from the defaults so earlier options do not leak in', () => {
    const noKey = QUERY_PRESETS.find((p) => p.id === 'no-key')!;
    const latest = QUERY_PRESETS.find((p) => p.id === 'whats-new')!;

    const afterNoKey = applyPreset(DEFAULT_QUERY, noKey);
    expect(afterNoKey.sendApiKey).toBe(false);
    expect(applyPreset(afterNoKey, latest).sendApiKey).toBe(true);
  });

  it('has unique ids', () => {
    expect(new Set(QUERY_PRESETS.map((p) => p.id)).size).toBe(QUERY_PRESETS.length);
  });
});
