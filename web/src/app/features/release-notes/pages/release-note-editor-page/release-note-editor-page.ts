import { ChangeDetectionStrategy, Component, computed, inject, input, signal } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ButtonDirective, ButtonIcon, ButtonLabel } from 'primeng/button';
import { Card } from 'primeng/card';
import { Message } from 'primeng/message';
import { Observable, finalize, firstValueFrom, map, of } from 'rxjs';
import { MediaApi } from '../../../../core/api/media-api';
import { ReleaseNotesApi } from '../../../../core/api/release-notes-api';
import { ReleaseNote, SaveReleaseNoteRequest } from '../../../../core/models/release-note.model';
import { NotificationService } from '../../../../core/notifications/notification.service';
import { ConfirmService } from '../../../../shared/services/confirm.service';
import { LoadingState } from '../../../../shared/ui/loading-state/loading-state';
import { ReleaseNoteForm } from '../../components/release-note-form/release-note-form';
import { ReleaseNotePreview } from '../../components/release-note-preview/release-note-preview';
import { ImageUploader } from '../../components/rich-text-editor/editor-image-upload';
import { toPreviewNote } from '../../preview/preview-note';

/** Creates a release note, or edits one when a release note id is in the route. */
@Component({
  selector: 'app-release-note-editor-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    RouterLink,
    ButtonDirective,
    ButtonIcon,
    ButtonLabel,
    Card,
    Message,
    LoadingState,
    ReleaseNoteForm,
    ReleaseNotePreview,
  ],
  templateUrl: './release-note-editor-page.html',
  styleUrl: './release-note-editor-page.scss',
})
export class ReleaseNoteEditorPage {
  private readonly api = inject(ReleaseNotesApi);
  private readonly mediaApi = inject(MediaApi);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly notifications = inject(NotificationService);
  private readonly confirmation = inject(ConfirmService);

  readonly applicationId = input.required<string>();
  readonly releaseNoteId = input<string>();

  protected readonly isNew = computed(() => !this.releaseNoteId());
  protected readonly saving = signal(false);

  /** The unsaved form values, updated as the user types. */
  protected readonly draft = signal<SaveReleaseNoteRequest>({
    version: '',
    title: '',
    contentHtml: '',
    isPublished: false,
  });

  /** Uploads editor images to this application's media and returns the URL to embed. */
  protected readonly uploadImage = computed<ImageUploader>(() => {
    const applicationId = this.applicationId();
    return (file) =>
      firstValueFrom(this.mediaApi.upload(applicationId, file).pipe(map((media) => media.url)));
  });

  protected readonly note = rxResource<
    ReleaseNote | null,
    { applicationId: string; id: string | undefined }
  >({
    params: () => ({ applicationId: this.applicationId(), id: this.releaseNoteId() }),
    stream: ({ params }) => (params.id ? this.api.get(params.applicationId, params.id) : of(null)),
  });

  /** The draft as consuming apps will receive it, for the live preview. */
  protected readonly preview = computed(() =>
    toPreviewNote(this.draft(), this.note.value()?.publishedAt ?? null),
  );

  protected save(request: SaveReleaseNoteRequest): void {
    const id = this.releaseNoteId();
    const operation = id
      ? this.api.update(this.applicationId(), id, request)
      : this.api.create(this.applicationId(), request);

    this.run(operation, (saved) => {
      this.notifications.success(`Release notes for ${saved.version} saved.`);
      this.backToList();
    });
  }

  protected async delete(): Promise<void> {
    const id = this.releaseNoteId();
    if (!id) {
      return;
    }

    const confirmed = await this.confirmation.confirm({
      header: 'Delete release notes',
      message: 'Delete these release notes? This cannot be undone.',
      acceptLabel: 'Delete',
      danger: true,
    });
    if (confirmed) {
      this.run(this.api.delete(this.applicationId(), id), () => {
        this.notifications.success('Release notes deleted.');
        this.backToList();
      });
    }
  }

  private run<T>(operation: Observable<T>, onSuccess: (value: T) => void): void {
    this.saving.set(true);
    operation.pipe(finalize(() => this.saving.set(false))).subscribe(onSuccess);
  }

  private backToList(): void {
    this.router.navigate(['..'], { relativeTo: this.route });
  }
}
