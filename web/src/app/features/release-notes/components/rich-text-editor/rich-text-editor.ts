import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  forwardRef,
  inject,
  input,
} from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { QuillEditorComponent, QuillModules } from 'ngx-quill';
import type Quill from 'quill';
import { RELEASE_NOTE_CATEGORIES } from '../../../../core/models/release-note-category';
import { ReleaseNoteContent } from '../../../../core/models/release-note.model';
import { exitSectionOnEmptyEnter, registerCategoryFormat } from './category-format';
import { ImageUploader, enableImageUpload } from './editor-image-upload';
import { readNoteContent, toNoteDelta } from './note-content';

const EMPTY_CONTENT: ReleaseNoteContent = { summaryHtml: '', sections: [] };

/**
 * One rich text editor for a whole release note. The toolbar's section picker puts lines in a
 * category section (Added, Fixed…); untagged lines form the summary. The form value is the
 * note's summary and sections, each as semantic HTML.
 */
@Component({
  selector: 'app-rich-text-editor',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [QuillEditorComponent],
  templateUrl: './rich-text-editor.html',
  styleUrl: './rich-text-editor.scss',
  providers: [
    { provide: NG_VALUE_ACCESSOR, useExisting: forwardRef(() => RichTextEditor), multi: true },
  ],
})
export class RichTextEditor implements ControlValueAccessor {
  private readonly destroyRef = inject(DestroyRef);

  readonly placeholder = input('Describe what changed…');
  /** Uploads images added in the editor. Without one, images cannot be added. */
  readonly uploadImage = input<ImageUploader | null>(null);

  protected readonly categories = RELEASE_NOTE_CATEGORIES;
  protected readonly modules: QuillModules = {
    keyboard: { bindings: { 'exit section on empty enter': exitSectionOnEmptyEnter } },
  };

  private quill: Quill | null = null;
  private value = EMPTY_CONTENT;
  private disabled = false;
  private onChange: (value: ReleaseNoteContent) => void = () => undefined;
  private onTouched: () => void = () => undefined;

  constructor() {
    registerCategoryFormat();
  }

  writeValue(value: ReleaseNoteContent | null): void {
    this.value = value ?? EMPTY_CONTENT;
    if (this.quill) {
      this.load(this.quill);
    }
  }

  registerOnChange(fn: (value: ReleaseNoteContent) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(disabled: boolean): void {
    this.disabled = disabled;
    this.quill?.enable(!disabled);
  }

  protected editorCreated(quill: Quill): void {
    this.quill = quill;
    this.load(quill);
    quill.enable(!this.disabled);

    const textChanged = (_delta: unknown, _old: unknown, source: string) => {
      if (source !== 'silent') {
        this.value = readNoteContent(quill);
        this.onChange(this.value);
      }
    };
    const selectionChanged = (range: unknown) => {
      if (!range) {
        this.onTouched();
      }
    };

    quill.on('text-change', textChanged);
    quill.on('selection-change', selectionChanged);
    const disableImageUpload = enableImageUpload(quill, () => this.uploadImage());

    this.destroyRef.onDestroy(() => {
      quill.off('text-change', textChanged);
      quill.off('selection-change', selectionChanged);
      disableImageUpload();
      this.quill = null;
    });
  }

  private load(quill: Quill): void {
    quill.setContents(toNoteDelta(quill, this.value), 'silent');
    quill.history.clear();
  }
}
