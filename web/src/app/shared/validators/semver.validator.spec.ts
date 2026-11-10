import { FormControl } from '@angular/forms';
import { isPreRelease, isSemVer, semverValidator } from './semver.validator';

describe('semver validator', () => {
  it.each(['1.0.0', '0.1.0', '1.0.0-beta.1', '2.3.4+build.5', '1.0.0-rc.1+sha.abc'])(
    'accepts %s',
    (value) => {
      expect(isSemVer(value)).toBe(true);
    },
  );

  it.each(['1', '1.0', 'v1.0.0', '01.0.0', '1.0.0-', '1.0.0.0'])('rejects %s', (value) => {
    expect(isSemVer(value)).toBe(false);
  });

  it('detects pre-releases, ignoring build metadata', () => {
    expect(isPreRelease('1.0.0-beta')).toBe(true);
    expect(isPreRelease('1.0.0+build-1')).toBe(false);
  });

  it('reports a semver error on the control', () => {
    expect(semverValidator(new FormControl('1.0'))).toEqual({ semver: true });
    expect(semverValidator(new FormControl('1.0.0'))).toBeNull();
    expect(semverValidator(new FormControl(''))).toBeNull();
  });
});
