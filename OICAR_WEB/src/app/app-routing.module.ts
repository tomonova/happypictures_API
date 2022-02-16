import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CardMakerComponent } from './screens/card-maker/card-maker.component';
import { HomeScreenComponent } from './screens/home-screen/home-screen.component';
import { LoginComponent } from './screens/login/login.component';
import { PictureGalleryComponent } from './screens/picture-gallery/picture-gallery.component';
import { SchedulerComponent } from './screens/scheduler/scheduler.component';
import { StoryMakerComponent } from './screens/story-maker/story-maker.component';

const routes: Routes = [{path:"",component:LoginComponent},
{path:"home",component:HomeScreenComponent},
{path:"card-maker",component:CardMakerComponent},
{path:"story-maker",component:StoryMakerComponent},
{path:"picture-gallery",component:PictureGalleryComponent},
{path:"scheduler",component:SchedulerComponent}];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: true })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
