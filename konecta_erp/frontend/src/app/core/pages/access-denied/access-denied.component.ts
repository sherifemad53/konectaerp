import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-access-denied',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="min-h-screen flex items-center justify-center bg-gray-100">
      <div class="max-w-md w-full bg-white shadow-lg rounded-lg p-8 text-center">
        <div class="text-6xl mb-4">🚫</div>
        <h1 class="text-2xl font-bold text-gray-800 mb-4">Access Denied</h1>
        <p class="text-gray-600 mb-8">
          You do not have permission to access this page.
          Please contact your administrator if you believe this is an error.
        </p>
        <div class="space-y-4">
          <a routerLink="/auth/login" class="block w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 px-4 rounded transition duration-200">
            Back to Login
          </a>
        </div>
      </div>
    </div>
  `,
  styles: []
})
export class AccessDeniedComponent {}
