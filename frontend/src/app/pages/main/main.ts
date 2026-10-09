import { Component, afterNextRender, ChangeDetectorRef } from '@angular/core';
import { Router } from "@angular/router";
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-main',
  imports: [FormsModule],
  templateUrl: `./main.html`,
  styles: ``,
})
export class Main {
  username = '' ;
  userId = '';
  search = '';
  tasks: any[] = [];
  categories: any[] = [];
  page = 0;

  selectedCategoryId: number | null = null;
  selectedTaskId = 0;
  selectedTask: any = null;

  showCategoryEditor = false;
  newCategoryName = '';

  constructor(private router: Router, private http: HttpClient, private cdr: ChangeDetectorRef){
    afterNextRender(() => {
      const storageUsername = localStorage.getItem("username");
      this.username = storageUsername ? storageUsername : "Username";
      this.cdr.detectChanges();
    });
  }

  ngOnInit(): void {
    this.loadCategories();
    this.loadTasks();
  }

  // 
  loadTasks(isNewSearch: boolean = false){
    if (!isNewSearch) {
      this.page += 1;
    }

    const tasksRequest : any={
      pageNumber: this.page
    }

    if (this.search.trim())
      tasksRequest.searchString = this.search.trim();

    if(this.selectedCategoryId !== null)
      tasksRequest.categoryId = this.selectedCategoryId;

    console.log(this.selectedCategoryId)

        this.http.get("http://localhost:5291/api/Task", { params: tasksRequest }).subscribe({
      next:(response: any)=>{
        
        if(this.page === 1)
          this.tasks = response;
        else
          this.tasks = [...this.tasks, ...response]

        this.cdr.detectChanges();
      },
      error: (err) => console.error(err)
    });
  }

  loadCategories(){
    this.http.get("http://localhost:5291/api/Category").subscribe({
      next:(response: any)=>{
        this.categories = response;

        this.cdr.detectChanges();
      },
      error: (err) => console.error(err)
    });
  }

  // 
  onTask(taskId: number){
    if (this.selectedTaskId === taskId) {
      this.onCancel();
      return;
    }

    this.selectedTaskId = taskId;

    const originalTask = this.tasks.find(u => u.id === taskId);
    
    if (originalTask)
      this.selectedTask = JSON.parse(JSON.stringify(originalTask));
  }

  onCancel(){
    this.selectedTaskId = 0; 
    this.selectedTask = null;
  }

  onSave() {
    if (!this.selectedTask) return;

    const request = {
      name: this.selectedTask.name, 
      description: this.selectedTask.description,
      categoryId: this.selectedTask.category?.id || this.selectedTask.categoryId || null
    };

    if (this.selectedTask.id === 0) {
      this.http.post('http://localhost:5291/api/Task', request).subscribe({
        next: (response: any) => {
          if (!response.category && this.selectedTask.category) {
             response.category = this.selectedTask.category;
          }
          this.tasks.unshift(response); 
          this.onCancel();
          this.cdr.detectChanges();
        },
        error: (err) => console.error(err)
      });
    } else {
      this.http.put(`http://localhost:5291/api/Task/${this.selectedTask.id}`, request).subscribe({
        next: (response: any) => {
          const index = this.tasks.findIndex(t => t.id === this.selectedTask.id);
          if (index !== -1) {
            this.tasks[index] = this.selectedTask;
          }
          this.onCancel();
          this.cdr.detectChanges();
        },
        error: (err) => console.error(err)
      });
    }
  }

  onLeave(){
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    this.router.navigate(["/login"]);
  }

  onDelete(){
    if (!this.selectedTask) return;
    
    this.http.delete(`http://localhost:5291/api/Task/${this.selectedTask.id}`).subscribe({
      next: (response: any) => {
        this.tasks = this.tasks.filter(t => t.id !== this.selectedTask.id);
        
        this.onCancel();
        
        this.cdr.detectChanges();
      },
      error: (err) => console.error(err)
    });
  }
  
  onCreateTask() {
    this.selectedTaskId = 0;
    this.selectedTask = {
      id: 0,
      name: '',
      description: '',
      category: null,
      categoryId: this.selectedCategoryId
    };
  }

  onSearch() {
    this.page = 1
    this.onCancel();
    this.loadTasks(true);
  }

  onCategorySelect(categoryId: number | null){
    this.selectedCategoryId = categoryId;
    this.page = 1;
    this.onCancel();
    this.loadTasks(true);
  }

  onCategoryChange(newCategoryId: number | null) {
    if (newCategoryId === null) {
      this.selectedTask.category = null;
      this.selectedTask.categoryId = null;
      return;
    }

    const selectedCat = this.categories.find(c => c.id === newCategoryId);
    if (selectedCat) {
      this.selectedTask.category = selectedCat;
      this.selectedTask.categoryId = selectedCat.id; 
    }
  }

  // 
  addCategory() {
    if (!this.newCategoryName.trim()) return;

    const request = { name: this.newCategoryName };

    this.http.post("http://localhost:5291/api/Category", request).subscribe({
      next: (response: any) => {
        this.categories.push(response);
        this.newCategoryName = '';
        this.cdr.detectChanges();
      },
      error: (err) => console.error(err)
    });
  }

  updateCategory(category: any) {
    const request = { name: category.name };

    this.http.put(`http://localhost:5291/api/Category/${category.id}`, request).subscribe({
      next: () => {
        this.tasks.forEach(t => {
          if (t.category?.id === category.id) {
            t.category.name = category.name;
          }
        });
        
        if (this.selectedTask?.category?.id === category.id) {
          this.selectedTask.category.name = category.name;
        }

        this.cdr.detectChanges();
      },
      error: (err) => console.error(err)
    });
  }

  deleteCategory(categoryId: number) {
    this.http.delete(`http://localhost:5291/api/Category/${categoryId}`).subscribe({
      next: () => {
        this.categories = this.categories.filter(c => c.id !== categoryId);
        
        this.tasks.forEach(t => {
          if (t.category?.id === categoryId) {
            t.category = null;
            t.categoryId = null;
          }
        });

        if (this.selectedCategoryId === categoryId) {
          this.onCategorySelect(null);
        }

        this.cdr.detectChanges();
      },
      error: (err) => console.error(err)
    });
  }
}