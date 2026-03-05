import { Component, inject, output, signal } from '@angular/core';
import { Auth } from '@core/auth';

@Component({
  selector: 'app-invite-popup',
  templateUrl: './invite-popup.html',
  styleUrl: './invite-popup.scss',
})
export class InvitePopup {
  private auth = inject(Auth);

  close = output<void>();

  loading = signal(true);
  inviteUrl = signal<string | null>(null);
  copied = signal(false);

  constructor() {
    this.auth.criaInvite().subscribe({
      next: (token) => {
        this.inviteUrl.set(`${window.location.origin}/register?token=${token}`);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  copy() {
    const url = this.inviteUrl();
    if (!url) return;
    navigator.clipboard.writeText(url).then(() => {
      this.copied.set(true);
      setTimeout(() => this.copied.set(false), 2000);
    });
  }
}
