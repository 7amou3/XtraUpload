import { Directive, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ComponentBase } from 'app/shared/components';
import { takeUntil } from 'rxjs/operators';

@Directive()
export abstract class PageCommon extends ComponentBase implements OnInit{
  pageFormGroup: FormGroup;
  name = new FormControl('', [Validators.required, Validators.minLength(3)]);
  content = new FormControl('', [Validators.required]);
  visibleInFooter = new FormControl(false);
  tooltipInfo: string;
  editorOptions = {
    toolbar: ["heading-1", "heading-2", "heading-3", "|", "bold", "italic", "heading", "|", "unordered-list", "ordered-list", "quote", "|", "link", "image", "table","|", "preview", "side-by-side", "fullscreen"],
    toolbarTips: false,
    status: false
  };
 
 constructor() {
     super();
     
 }
  ngOnInit(): void {
    
      this.visibleInFooter.valueChanges.pipe(takeUntil(this.onDestroy)).subscribe((visible: boolean) => {
        
        this.tooltipInfo = visible 
                            ? $localize`A link to this page will be visible in the main footer.`
                            : $localize`The link of this page will not be visible in the main footer`;
      });
      this.Init();
  }
  /** Invoked when component is ready */
  protected abstract Init();
}