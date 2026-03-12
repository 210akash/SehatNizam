import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-document-viewer',
  templateUrl: './document-viewer.component.html',
  styleUrl:    './document-viewer.component.css',
  standalone: false,
})
export class DocumentViewerComponent {
  urlSafe!: SafeResourceUrl;
  fileType: 'image' | 'pdf' | 'other' = 'other';
  isLoading = false;
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { path: string },
    private sanitizer: DomSanitizer
  ) {
    this.processPath(data.path);
  }

  private processPath(path: string): void {
    this.isLoading = true;
    const extension = (path.split('.').pop() || '').toLowerCase();
    if (['jpg', 'jpeg', 'png', 'gif', 'webp'].includes(extension)) {
      this.fileType = 'image';
      this.urlSafe = path;
    } else if (extension === 'pdf') {
      this.fileType = 'pdf';
      this.urlSafe = this.sanitizer.bypassSecurityTrustResourceUrl(path + '#toolbar=0');
    } else {
      this.fileType = 'other';
      this.urlSafe = this.sanitizer.bypassSecurityTrustResourceUrl(path);
    }
    this.isLoading = false;
  }
}
