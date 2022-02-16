import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './screens/login/login.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CardMakerComponent } from './screens/card-maker/card-maker.component';
import { StoryMakerComponent } from './screens/story-maker/story-maker.component';
import { PictureGalleryComponent } from './screens/picture-gallery/picture-gallery.component';
import { SchedulerComponent } from './screens/scheduler/scheduler.component';
import { HeaderComponent } from './components/header/header.component';

import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ChoosePictureDialogComponent } from './components/choose-picture-dialog/choose-picture-dialog.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
//Angular Material stuff
import { MatInputModule } from '@angular/material/input';
import {MatTabsModule} from '@angular/material/tabs';
import {MatSnackBarModule} from '@angular/material/snack-bar';
import {MatPaginatorModule} from '@angular/material/paginator';
import {MatCheckboxModule} from '@angular/material/checkbox';
import { HttpClientModule } from '@angular/common/http';
import { CardListComponent } from './components/card-list/card-list.component';
import { CardViewerComponent } from './components/card-viewer/card-viewer.component';
import {TextFieldModule} from '@angular/cdk/text-field';
import { AddNewImageDialogComponent } from './components/add-new-image-dialog/add-new-image-dialog.component';
import { StoriesListComponent } from './components/stories-list/stories-list.component';
import { HomeScreenComponent } from './screens/home-screen/home-screen.component';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';
import { StoryViewerComponent } from './components/story-viewer/story-viewer.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    CardMakerComponent,
    StoryMakerComponent,
    PictureGalleryComponent,
    SchedulerComponent,
    HeaderComponent,
    ChoosePictureDialogComponent,
    CardListComponent,
    CardViewerComponent,
    AddNewImageDialogComponent,
    StoriesListComponent,
    HomeScreenComponent,
    ConfirmDialogComponent,
    StoryViewerComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatDialogModule,
    MatPaginatorModule,
    FormsModule,
    MatInputModule,
    MatTabsModule,
    MatSnackBarModule,
    MatCheckboxModule,
    ReactiveFormsModule,
    HttpClientModule,
    TextFieldModule
  ],
  providers: [  { provide: MatDialogRef, useValue: {} },
    { provide: MAT_DIALOG_DATA, useValue: [] }],
  bootstrap: [AppComponent]
})
export class AppModule { }
