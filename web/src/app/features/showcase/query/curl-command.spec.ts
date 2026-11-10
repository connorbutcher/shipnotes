import { buildCurlCommand, maskApiKey } from './curl-command';

describe('curl command', () => {
  it('includes the key header only when a key is sent', () => {
    expect(buildCurlCommand('http://x/y', 'sn_secret')).toBe(
      'curl "http://x/y" \\\n  -H "X-Api-Key: sn_secret"',
    );
    expect(buildCurlCommand('http://x/y', null)).toBe('curl "http://x/y"');
  });

  it('masks all but the start of a key', () => {
    expect(maskApiKey('sn_abcdefghijklmnop')).toBe('sn_abcdefghi…');
    expect(maskApiKey('short')).toBe('short');
  });
});
