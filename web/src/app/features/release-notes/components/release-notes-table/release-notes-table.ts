import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TableModule } from 'primeng/table';
import { Tag } from 'primeng/tag';
import { ReleaseNoteSummary } from '../../../../core/models/release-note.model';

const PAGE_SIZE = 10;

/** Release notes for one application, newest version first. Rows open the editor. */
@Component({
  selector: 'app-release-notes-table',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, RouterLink, TableModule, Tag],
  templateUrl: './release-notes-table.html',
  styleUrl: './release-notes-table.scss',
})
export class ReleaseNotesTable {
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly notes = input.required<ReleaseNoteSummary[]>();

  protected readonly pageSize = PAGE_SIZE;

  protected open(note: ReleaseNoteSummary): void {
    this.router.navigate([note.id], { relativeTo: this.route });
  }
}
