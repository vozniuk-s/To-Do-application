import { Component } from '@angular/core';
import { Router } from "@angular/router";
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: `./login.html`,
  styles: ``,
})
export class Login {
  username = '';
  password = '';

  isPasswordHidden = true;

  constructor(private router: Router, private http: HttpClient){}

  onLogin(){
    const loginRequest={
      name: this.username,
      password: this.password
    };

    this.http.post('http://localhost:5291/api/Auth/login', loginRequest).subscribe({
      next: (response: any)=>{
        localStorage.setItem('token', response.token);
        localStorage.setItem('username', this.username);
        this.router.navigate(['/main']);
      },
      error: (err) => {
        console.error(err);
        alert("Wrong login or password");
      } 
    });
  }

  onRegister(){
    const registerRequest={
      name: this.username,
      password: this.password
    }

    this.http.post('http://localhost:5291/api/Auth/register', registerRequest).subscribe({
      next: (response: any)=>{
        this.onLogin();
      },
      error: (err) => {
        console.error(err);
        alert("Wrong login or password");
      } 
    })
  }

  onCheckBox(){
    this.isPasswordHidden = !this.isPasswordHidden;
  }
}