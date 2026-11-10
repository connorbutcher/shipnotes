import { DOCUMENT } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { Card } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { Tab, TabList, TabPanel, TabPanels, Tabs } from 'primeng/tabs';
import { API_BASE_URL } from '../../../../core/api/api-config';
import { CopyButton } from '../../../../shared/ui/copy-button/copy-button';

interface Endpoint {
  path: string;
  description: string;
}

interface Snippet {
  title: string;
  code: string;
}

/** Shows consuming apps how to fetch and render this application's release notes. */
@Component({
  selector: 'app-integration-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [Card, TableModule, Tabs, TabList, Tab, TabPanels, TabPanel, CopyButton],
  templateUrl: './integration-page.html',
  styleUrl: './integration-page.scss',
})
export class IntegrationPage {
  private readonly origin = inject(API_BASE_URL) || inject(DOCUMENT).location.origin;

  protected readonly endpoints: Endpoint[] = [
    {
      path: 'GET /api/v1/public/release-notes',
      description:
        'Published notes, newest first. Query: after, upTo, includePreReleases, limit (1-100).',
    },
    {
      path: 'GET /api/v1/public/release-notes/latest',
      description: 'The newest published note. Query: includePreReleases.',
    },
    {
      path: 'GET /api/v1/public/release-notes/{version}',
      description: 'The note for one version, e.g. 1.4.0.',
    },
    { path: 'GET /api/v1/public/application', description: 'The application name and icon URL.' },
  ];

  protected readonly snippets = computed<Snippet[]>(() => [
    {
      title: 'curl',
      code: `curl -H "X-Api-Key: $SHIPNOTES_API_KEY" \\\n  "${this.origin}/api/v1/public/release-notes?after=1.2.0"`,
    },
    {
      title: 'JavaScript',
      code: [
        `const response = await fetch('${this.origin}/api/v1/public/release-notes?limit=10', {`,
        `  headers: { 'X-Api-Key': SHIPNOTES_API_KEY },`,
        `});`,
        `const notes = await response.json();`,
        ``,
        `// contentHtml is sanitised by ShipNotes and safe to render.`,
        `container.innerHTML = notes`,
        `  .map((n) => \`<article><h2>\${n.version}: \${n.title}</h2>\${n.contentHtml}</article>\`)`,
        `  .join('');`,
      ].join('\n'),
    },
  ]);
}
