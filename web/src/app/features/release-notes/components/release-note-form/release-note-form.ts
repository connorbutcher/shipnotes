import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  input,
  output,
} from '@angular/core';
import { outputFromObservable, toSignal } from '@angular/core/rxjs-interop';
import { FormArray, NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonDirective, ButtonIcon, ButtonLabel } from 'primeng/button';
import { InputText } from 'primeng/inputtext';
import { Message } from 'primeng/message';
import { Tag } from 'primeng/tag';
import { ToggleSwitch } from 'primeng/toggleswitch';
import { map, startWith } from 'rxjs';
import { ReleaseNote, SaveReleaseNoteRequest } from '../../../../core/models/release-note.model';
import { isPreRelease, semverValidator } from '../../../../shared/validators/semver.validator';
import { normalizeEditorHtml } from '../../../../shared/utils/editor-html';
import { SectionForm, setSections, toSectionRequests } from '../../forms/section-form';
import { ReleaseNoteSections } from '../release-note-sections/release-note-sections';
import { ImageUploader } from '../rich-text-editor/editor-image-upload';
import { RichTextEditor } from '../rich-text-editor/rich-text-editor';

const TITLE_MAX_LENGTH = 200;

@Component({
  selector: 'app-release-note-form',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    ReactiveFormsModule,
    ButtonDirective,
    ButtonIcon,
    ButtonLabel,
    InputText,
    Message,
    Tag,
    ToggleSwitch,
    RichTextEditor,
    ReleaseNoteSections,
  ],
  templateUrl: './release-note-form.html',
  styleUrl: './release-note-form.scss',
})
export class ReleaseNoteForm {
  private readonly fb = inject(NonNullableFormBuilder);

  readonly releaseNote = input<ReleaseNote | null>(null);
  readonly saving = input(false);
  readonly uploadImage = input<ImageUploader | null>(null);
  readonly saved = output<SaveReleaseNoteRequest>();

  protected readonly titleMaxLength = TITLE_MAX_LENGTH;

  protected readonly form = this.fb.group({
    version: ['', [Validators.required, semverValidator]],
    title: ['', [Validators.required, Validators.maxLength(TITLE_MAX_LENGTH)]],
    summaryHtml: [''],
    sections: new FormArray<SectionForm>([]),
    isPublished: [false],
  });

  /** The form's current values on every change, for the live preview. */
  readonly draftChanged = outputFromObservable(
    this.form.valueChanges.pipe(
      startWith(null),
      map(() => this.toRequest()),
    ),
  );

  private readonly version = toSignal(this.form.controls.version.valueChanges, {
    initialValue: '',
  });
  protected readonly preRelease = computed(() => isPreRelease(this.version()));

  constructor() {
    effect(() => {
      const note = this.releaseNote();
      if (note) {
        setSections(this.form.controls.sections, note.sections);
        this.form.reset({
          version: note.version,
          title: note.title,
          summaryHtml: note.summaryHtml,
          isPublished: note.isPublished,
        });
      }
    });
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saved.emit(this.toRequest());
  }

  private toRequest(): SaveReleaseNoteRequest {
    const value = this.form.getRawValue();
    return {
      version: value.version.trim(),
      title: value.title.trim(),
      summaryHtml: normalizeEditorHtml(value.summaryHtml),
      sections: toSectionRequests(value.sections),
      isPublished: value.isPublished,
    };
  }
}
