import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { jwtDecode } from 'jwt-decode';
import { environment } from '../../../environments/environment';
import { map, Observable, tap } from 'rxjs';

interface ApiResponse<T> {
  result?: T;
  Result?: T;
  code?: string;
  Code?: string;
  c_Message?: string;
  C_Message?: string;
  s_Message?: string;
  S_Message?: string;
}

interface LoginRequest {
  email: string;
  password: string;
}

interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
}

interface UpdatePasswordRequest {
  oldPassword: string;
  newPassword: string;
  confirmPassword: string;
}

interface LoginResult {
  accessToken: string;
  expiresAtUtc: string;
  keyId: string;
  userId: string;
  email: string;
  roles: string[];
  permissions?: string[];
}

export interface AuthSession {
  token: string;
  userId: string;
  email: string;
  fullName?: string;
  roles: string[];
  permissions: string[];
  expiresAtUtc: string;
}

const STORAGE_KEY = 'konecta.erp.session';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly sessionSignal = signal<AuthSession | null>(this.restoreSession());

  readonly currentSession = computed(() => this.sessionSignal());
  readonly isAuthenticated = computed(() => {
    const session = this.sessionSignal();
    if (!session) {
      return false;
    }
    return new Date(session.expiresAtUtc).getTime() > Date.now();
  });

  login(request: LoginRequest) {
    const url = `${environment.apiBaseUrl}${environment.endpoints.auth}/login`;
    return this.http.post<ApiResponse<LoginResult>>(url, request)
      .pipe(
        this.unwrapResponse<LoginResult>(),
        this.persistSession()
      );
  }

  register(request: RegisterRequest) {
    const url = `${environment.apiBaseUrl}${environment.endpoints.auth}/register`;
    return this.http.post<ApiResponse<unknown>>(url, request)
      .pipe(this.unwrapResponse());
  }

  validateToken(token: string) {
    const url = `${environment.apiBaseUrl}${environment.endpoints.auth}/validate-token`;
    return this.http.post<ApiResponse<{ email: string }>>(url, token)
      .pipe(this.unwrapResponse());
  }

  updatePassword(request: UpdatePasswordRequest) {
    const url = `${environment.apiBaseUrl}${environment.endpoints.auth}/update-password`;
    return this.http.put<ApiResponse<{ email: string }>>(url, request)
      .pipe(this.unwrapResponse());
  }

  logout(): void {
    localStorage.removeItem(STORAGE_KEY);
    this.sessionSignal.set(null);
  }

  currentToken(): string | null {
    const session = this.sessionSignal();
    return session ? session.token : null;
  }

  hasPermission(permission: string): boolean {
    const session = this.sessionSignal();
    return !!session?.permissions?.includes(permission);
  }

  hasRole(role: string): boolean {
    const session = this.sessionSignal();
    return !!session?.roles?.some(r => r.toLowerCase() === role.toLowerCase());
  }

  private unwrapResponse<T>() {
    return (source: Observable<ApiResponse<T>>) =>
      source.pipe(
        map(response => {
          console.log('unwrapResponse - raw response:', response);
          const result = response.result ?? (response as any).Result ?? (response as any).data ?? null;
          console.log('unwrapResponse - extracted result:', result);
          return result as T;
        })
      );
  }

  private persistSession() {
    return (source: Observable<LoginResult>) =>
      source.pipe(
        tap(result => {
          console.log('Login response received:', result);
          // Handle both camelCase (frontend expected) and PascalCase (backend default)
          const r = result as any;
          const accessToken = r.accessToken || r.AccessToken;
          const expiresAtUtc = r.expiresAtUtc || r.ExpiresAtUtc;
          const userId = r.userId || r.UserId;
          const email = r.email || r.Email;
          const roles = r.roles || r.Roles || [];
          const permissions = r.permissions || r.Permissions || [];

          const session: AuthSession = {
            token: accessToken,
            userId: userId,
            email: email,
            roles: roles,
            permissions: permissions,
            expiresAtUtc: expiresAtUtc,
            fullName: this.extractFullName(accessToken)
          };
          localStorage.setItem(STORAGE_KEY, JSON.stringify(session));
          this.sessionSignal.set(session);
        })
      );
  }

  private restoreSession(): AuthSession | null {
    const cached = localStorage.getItem(STORAGE_KEY);
    if (!cached) {
      return null;
    }

    try {
      const parsed = JSON.parse(cached) as AuthSession;
      if (new Date(parsed.expiresAtUtc).getTime() < Date.now()) {
        localStorage.removeItem(STORAGE_KEY);
        return null;
      }
      return parsed;
    } catch {
      localStorage.removeItem(STORAGE_KEY);
      return null;
    }
  }

  private extractFullName(token: string): string | undefined {
    try {
      const decoded = jwtDecode<{ full_name?: string }>(token);
      return decoded.full_name;
    } catch {
      return undefined;
    }
  }
}
