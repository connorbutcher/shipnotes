# ShipNotes

ShipNotes lets you write release notes for your applications and serve them to those applications through an API.

- **Applications** (name, icon URL, API key) are listed in the side panel.
- Each application has **release notes per semantic version**, written in a rich text editor and saved as draft or published.
- Consuming apps fetch **published** notes with the application's **API key** and render the HTML.

![Release notes list](docs/release-notes.png)

| Part | Stack | Folder |
| --- | --- | --- |
| Database | SQL Server, schema managed by EF Core migrations | `api/src/ShipNotes.Infrastructure/Persistence/Migrations`, `database/shipnotes.sql` |
| API | .NET 10 ASP.NET Core Web API | `api/` |
| UI | Angular 22 (standalone components, signals), PrimeNG 22 with the Aura theme, ngx-quill | `web/` |

## Running locally

Prerequisites: .NET 10 SDK, Node 22, and Docker (or any SQL Server instance).

```bash
# 1. Database
docker compose up -d

# 2. API on http://localhost:5078 (applies migrations automatically in Development)
cd api
dotnet run --project src/ShipNotes.Api

# 3. UI on http://localhost:4200 (proxies /api to the API)
cd web
npm install
npm start
```

The development connection string is in `api/src/ShipNotes.Api/appsettings.Development.json`. For other environments set `ConnectionStrings__ShipNotes` and `Cors__AdminOrigins__0`, and apply the schema with `database/shipnotes.sql` (an idempotent script) or `dotnet ef database update`.

## Images in release notes

Images are never stored inline in a note. The editor's image button (and pasting or dropping an image file) uploads the file to `POST /api/v1/applications/{id}/media`, which saves it to disk and inserts its URL, e.g. `https://your-api/api/v1/media/{mediaId}/screenshot.png`, into the note. Each upload is tracked in the `MediaData` table (file name, type, size, storage path and the release note that uses it).

- Files are written to `Media:RootPath` (default `F:\Dev\ShipNotesMedia`) as `{applicationId}/{mediaId}.{ext}`.
- Media URLs use `Media:PublicBaseUrl` when set (set it in production, e.g. `Media__PublicBaseUrl=https://notes.example.com`); otherwise the API's own address.
- Only PNG, JPEG, GIF and WebP up to `Media:MaxFileSizeBytes` (5 MB) are accepted; the type is checked from the file's content, not its name.
- When a note is saved the server keeps only `<img>` tags pointing at media uploaded to that application, so base64 and hot-linked images are removed. Removing an image from a note, or deleting the note or application, deletes the files too.
- `GET /api/v1/media/{mediaId}/{fileName}` is anonymous and cacheable, so consuming apps can render the note HTML as-is and the images load.

## Querying release notes from your app

Every request must send the application's API key in the `X-Api-Key` header, and the URL names the application the key was issued to. The key is checked against that application on every call:

- no key, or an unknown key: `401 Unauthorized`
- a valid key for a different application (or an application id that doesn't exist): `403 Forbidden`

The application id is shown on the application's **Integration** tab. Only published notes are returned, newest version first. Paths below are relative to `/api/v1/public/applications/{applicationId}`.

| Endpoint | Description |
| --- | --- |
| `GET /release-notes` | List notes. Query: `after` (exclusive), `upTo` (inclusive), `includePreReleases` (default `false`), `limit` (1–100, default 20). |
| `GET /release-notes/between` | All notes between two versions. Query: `from` (required, exclusive), `to` (required, inclusive), `includeFrom` (default `false`; set `true` to include `from` itself), `includePreReleases` (default `false`). Returns `400` if `from` is later than `to`. |
| `GET /release-notes/latest` | Newest published note. Query: `includePreReleases`. |
| `GET /release-notes/{version}` | Note for one version, e.g. `1.4.0`. |
| `GET /` | Application name and icon URL. |

Versions are compared by [semver precedence](https://semver.org/#spec-item-11), not as text: `1.10.0` is after `1.9.0`, `2.0.0-beta.1` is before `2.0.0`, and build metadata (`+build.5`) is ignored.

`between` excludes `from` and includes `to` because the usual question is "the user had 1.2.0 and now has 1.5.0, what changed?": they already saw 1.2.0's notes, and 1.5.0's notes are part of the answer.

```bash
# "What changed between the version this user last saw and the one they have now?"
curl -H "X-Api-Key: $SHIPNOTES_API_KEY" \
  "https://your-shipnotes-host/api/v1/public/applications/$SHIPNOTES_APP_ID/release-notes/between?from=1.2.0&to=1.5.0"
```

```json
[
  {
    "version": "1.5.0",
    "isPreRelease": false,
    "title": "Dark mode",
    "contentHtml": "<p>Highlights</p><ul><li>Dark mode</li></ul>",
    "publishedAt": "2026-11-10T20:00:45+00:00"
  }
]
```

`contentHtml` is sanitised on the server with an allow-list, so it can be rendered directly. The public endpoints allow any origin (CORS) and are rate limited to 120 requests per minute per key.

## Design notes

**API layers** (`api/src`), each project depending only on the one before it:

- `ShipNotes.Domain`: entities (`ClientApplication`, `ReleaseNote`) and the `SemanticVersion` value object. Versions are stored with a `VersionSortKey` whose ordinal order matches semver precedence, so sorting and range filters run in SQL on one indexed column.
- `ShipNotes.Application`: use-case services, DTOs, FluentValidation validators, and the interfaces infrastructure implements (repositories, unit of work, API key service, HTML sanitiser).
- `ShipNotes.Infrastructure`: EF Core `DbContext`, entity configurations, repositories, migrations, API key hashing, and HTML sanitisation.
- `ShipNotes.Api`: thin controllers, API key authentication handler, problem-details error handling, CORS, and rate limiting.

**API keys** are 256-bit random values (`sn_…`). Only a SHA-256 hash and a short display prefix are stored, so a key is shown once when it's created or regenerated.

**UI** (`web/src/app`):

- `core/`: models, API clients, the signal-based `ApplicationsStore`, notifications (PrimeNG toast), the HTTP error interceptor, and the PrimeNG theme (`core/theme`: Aura with an indigo primary palette).
- `layout/`: the shell, the side panel (PrimeNG menu), and the application nav item.
- `shared/`: reusable UI pieces built on PrimeNG (avatar icon, copy button, API key banner, empty and loading states), the confirm dialog service, the semver validator, and utilities.
- `features/applications` and `features/release-notes`: lazy-loaded routes, with pages that load data and handle navigation, and presentational components for forms and list items.

**PrimeNG licence.** From version 22, PrimeNG needs a licence key; without one it shows an "Invalid PrimeUI License" notice in the corner of the page. Individuals and small teams qualify for the free Community licence at https://primeui.dev/licenses. Put the key in `primeNgLicenseKey` in `web/src/environments/environment*.ts`.

**Admin endpoints** (`/api/v1/applications/...`) don't require sign-in yet. Put them behind your identity provider (for example, Entra ID or Auth0 with JWT bearer) before you deploy anywhere public.

## Tests and checks

```bash
cd api && dotnet test                  # domain, infrastructure and HTTP integration tests (SQLite in-memory)
cd web && npm run test:ci              # Vitest component, store and validator tests
cd web && npx ng lint && npm run format:check
```

CI runs all of these on every push and pull request (`.github/workflows/ci.yml`).
