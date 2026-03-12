import { Component, OnInit } from '@angular/core';
import { LoaderService } from '../../../Service/loader.service';

@Component({
    selector: 'app-my-loader',
    templateUrl: './my-loader.component.html',
    styleUrls: ['./my-loader.component.css'],
    standalone: false
})
export class MyLoaderComponent implements OnInit {
  isLoading: boolean = false;

  constructor(private loaderService: LoaderService) {

    this.loaderService.isLoading.subscribe((v) => {
      this.isLoading = v;
    });

  }

  ngOnInit(): void {
  }

}
