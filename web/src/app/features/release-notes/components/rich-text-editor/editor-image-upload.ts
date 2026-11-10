import type Quill from 'quill';
import { Delta } from 'quill';
import type Toolbar from 'quill/modules/toolbar';
import { isMediaUrl } from '../../../../shared/utils/media-url';

/** Uploads an image and resolves with the URL to place in the note. */
export type ImageUploader = (file: File) => Promise<string>;

export const UPLOADABLE_IMAGE_TYPES = ['image/png', 'image/jpeg', 'image/gif', 'image/webp'];

/**
 * Makes every way of adding an image to the editor go through the upload API,
 * so notes only ever contain links to uploaded media and never inline (base64) images:
 * - the toolbar image button opens a file picker and uploads the chosen file;
 * - pasted or dropped image files are uploaded instead of being inlined;
 * - pasted HTML keeps only images that already point at uploaded media.
 *
 * Works with any Quill 2 instance. Returns a function that removes the listeners.
 */
export function enableImageUpload(
  quill: Quill,
  getUploader: () => ImageUploader | null,
): () => void {
  const insertFiles = (files: File[]) => void uploadAndInsert(quill, files, getUploader());

  (quill.getModule('toolbar') as Toolbar | undefined)?.addHandler('image', () =>
    pickImage(quill.root.ownerDocument).then((file) => file && insertFiles([file])),
  );

  quill.clipboard.addMatcher('IMG', (node, delta) =>
    isMediaUrl((node as HTMLImageElement).getAttribute('src')) ? delta : new Delta(),
  );

  const interceptFiles = (event: Event, files: FileList | null | undefined) => {
    const all = Array.from(files ?? []);
    if (all.length === 0) {
      return;
    }

    // Stop Quill's default handling, which would inline the files as base64 data URIs.
    event.preventDefault();
    event.stopImmediatePropagation();
    insertFiles(all.filter((file) => UPLOADABLE_IMAGE_TYPES.includes(file.type)));
  };

  const onDrop = (event: DragEvent) => interceptFiles(event, event.dataTransfer?.files);
  const onPaste = (event: ClipboardEvent) => interceptFiles(event, event.clipboardData?.files);

  quill.root.addEventListener('drop', onDrop, { capture: true });
  quill.root.addEventListener('paste', onPaste, { capture: true });

  return () => {
    quill.root.removeEventListener('drop', onDrop, { capture: true });
    quill.root.removeEventListener('paste', onPaste, { capture: true });
  };
}

async function uploadAndInsert(
  quill: Quill,
  files: File[],
  uploader: ImageUploader | null,
): Promise<void> {
  if (!uploader) {
    return;
  }

  for (const file of files) {
    try {
      const url = await uploader(file);
      const index = quill.getSelection(true)?.index ?? quill.getLength();
      quill.insertEmbed(index, 'image', url, 'user');
      quill.setSelection(index + 1, 0, 'user');
    } catch {
      // The HTTP error interceptor already tells the user why the upload failed.
    }
  }
}

function pickImage(document: Document): Promise<File | null> {
  return new Promise((resolve) => {
    const input = document.createElement('input');
    input.type = 'file';
    input.accept = UPLOADABLE_IMAGE_TYPES.join(',');
    input.addEventListener('change', () => resolve(input.files?.[0] ?? null), { once: true });
    input.addEventListener('cancel', () => resolve(null), { once: true });
    input.click();
  });
}
