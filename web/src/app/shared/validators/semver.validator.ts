import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

/** Semantic Versioning 2.0.0 pattern from https://semver.org. */
const SEMVER_PATTERN =
  /^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$/;

export function isSemVer(value: string): boolean {
  return SEMVER_PATTERN.test(value.trim());
}

export function isPreRelease(value: string): boolean {
  return isSemVer(value) && value.trim().split('+')[0].includes('-');
}

export const semverValidator: ValidatorFn = (
  control: AbstractControl<string | null>,
): ValidationErrors | null => {
  const value = control.value;
  return !value || isSemVer(value) ? null : { semver: true };
};
