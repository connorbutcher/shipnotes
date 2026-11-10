/** A copy-pasteable curl command for a console request. */
export function buildCurlCommand(url: string, apiKey: string | null): string {
  const header = apiKey ? ` \\\n  -H "X-Api-Key: ${apiKey}"` : '';
  return `curl "${url}"${header}`;
}

/** Shows only the start of a key, e.g. on screen while the full key goes into copied commands. */
export function maskApiKey(apiKey: string, visible = 12): string {
  return apiKey.length > visible ? `${apiKey.slice(0, visible)}…` : apiKey;
}
