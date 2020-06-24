import { Routes } from '@angular/router';
import { FileComponent } from './file/file.component';
import { FolderComponent } from './folder/folder.component';
export const DownloadRoutes: Routes = [
    { path: 'file', component: FileComponent },
    { path: 'folder', component: FolderComponent }
];
